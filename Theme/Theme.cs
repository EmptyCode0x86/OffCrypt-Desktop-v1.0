using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;

namespace OffCrypt
{
    // KORJATTU THEME COMPONENT - Toimii sekä design että runtime aikana!
    [ToolboxItem(true)]
    [Description("OffCrypt Dark Theme - Apply modern dark theme to form")]
    public class OffCryptTheme : Component
    {
        private Form _parentForm;
        private Panel _titleBarPanel;
        private Button _closeButton, _maximizeButton, _minimizeButton;
        private Label _titleLabel;
        private bool _isDragging = false;
        private bool _isResizing = false;
        private Point _lastCursor;
        private Color _accentColor = Color.FromArgb(39, 174, 96);
        private bool _themeApplied = false;
        private ResizeDirection _resizeDirection = ResizeDirection.None;
        private const int RESIZE_BORDER_WIDTH = 8;

        // Layout management
        private Dictionary<Control, ControlLayout> _controlLayouts = new Dictionary<Control, ControlLayout>();
        private Size _originalFormSize;
        private bool _layoutInitialized = false;

        private class ControlLayout
        {
            public float XRatio { get; set; }
            public float YRatio { get; set; }
            public float WidthRatio { get; set; }
            public float HeightRatio { get; set; }
            public AnchorStyles OriginalAnchor { get; set; }
            public DockStyle OriginalDock { get; set; }
        }

        private enum ResizeDirection
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        [Category("Appearance")]
        [Description("Main accent color for the theme")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color AccentColor
        {
            get { return _accentColor; }
            set
            {
                _accentColor = value;
                if (_themeApplied) ApplyTheme();
            }
        }

        [Category("Appearance")]
        [Description("Form title text")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string FormTitle { get; set; } = "OffCrypt";

        // Design time setup
        public override ISite? Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;
                if (value != null && value.DesignMode)
                {
                    var host = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (host?.RootComponent is Form form)
                    {
                        SetupForm(form);
                    }
                }
            }
        }

        // Runtime setup - call this in Form_Load event
        public void AttachToForm(Form form)
        {
            SetupForm(form);
        }

        private void SetupForm(Form form)
        {
            if (form == null) return;

            _parentForm = form;

            // Jos form on jo näkyvissä (runtime), apply heti
            if (form.Created || !this.DesignMode)
            {
                ApplyTheme();
            }
            else
            {
                // Design time tai form ei vielä luotu
                form.Load += (s, e) => ApplyTheme();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveTheme();
            }
            base.Dispose(disposing);
        }

        public void ApplyTheme()
        {
            if (_parentForm == null || _themeApplied) return;

            try
            {
                // Poista vanha theme
                RemoveTheme();

                // Aseta form propertyilta
                _parentForm.BackColor = Color.FromArgb(31, 31, 31);
                _parentForm.ForeColor = Color.White;
                _parentForm.FormBorderStyle = FormBorderStyle.None;
                _parentForm.Padding = new Padding(2);

                // Luo custom title bar
                CreateCustomTitleBar();

                // Apply theme kaikkiin child controlleihin
                ApplyThemeToControls(_parentForm);

                // Lisää border paint event
                _parentForm.Paint += ParentForm_Paint;

                // Lisää resize event handlerit
                _parentForm.MouseMove += ParentForm_MouseMove;
                _parentForm.MouseDown += ParentForm_MouseDown;
                _parentForm.MouseUp += ParentForm_MouseUp;
                _parentForm.Resize += ParentForm_Resize;

                // Tallenna alkuperäiset layout tiedot
                InitializeLayout();

                _themeApplied = true;
            }
            catch (Exception ex)
            {
                // Design time error handling
                if (this.DesignMode)
                {
                    MessageBox.Show($"Theme error: {ex.Message}");
                }
            }
        }

        private void CreateCustomTitleBar()
        {
            // Luo title bar panel
            _titleBarPanel = new Panel
            {
                Height = 35,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(45, 45, 45),
                Cursor = Cursors.SizeAll
            };

            // Title label
            _titleLabel = new Label
            {
                Text = $"🔐 {FormTitle}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(10, 8),
                Size = new Size(300, 20),
                BackColor = Color.Transparent
            };

            // Window control buttons
            _closeButton = CreateWindowButton("×", Color.FromArgb(232, 17, 35));
            _closeButton.Click += (s, e) => _parentForm.Close();

            _maximizeButton = CreateWindowButton("□", Color.FromArgb(70, 70, 70));
            _maximizeButton.Click += MaximizeButton_Click;

            _minimizeButton = CreateWindowButton("_", Color.FromArgb(70, 70, 70));
            _minimizeButton.Click += (s, e) => _parentForm.WindowState = FormWindowState.Minimized;

            // Päivitä button sijainnit
            _parentForm.Resize += UpdateButtonPositions;
            UpdateButtonPositions(null, null);

            // Drag funktionaliteetti title labelille
            _titleLabel.MouseDown += TitleBar_MouseDown;
            _titleLabel.MouseMove += TitleBar_MouseMove;
            _titleLabel.MouseUp += TitleBar_MouseUp;
            _titleLabel.MouseDoubleClick += (s, e) => MaximizeButton_Click(s, e);

            // Drag funktionaliteetti title bar panelille
            _titleBarPanel.MouseDown += TitleBar_MouseDown;
            _titleBarPanel.MouseMove += TitleBar_MouseMove;
            _titleBarPanel.MouseUp += TitleBar_MouseUp;
            _titleBarPanel.MouseDoubleClick += (s, e) => MaximizeButton_Click(s, e);

            // Lisää kontrollit title bariin
            _titleBarPanel.Controls.Add(_titleLabel);
            _titleBarPanel.Controls.Add(_minimizeButton);
            _titleBarPanel.Controls.Add(_maximizeButton);
            _titleBarPanel.Controls.Add(_closeButton);

            // Lisää title bar formiin
            _parentForm.Controls.Add(_titleBarPanel);
            _titleBarPanel.BringToFront();
        }

        private void UpdateButtonPositions(object sender, EventArgs e)
        {
            if (_closeButton != null && _titleBarPanel != null)
            {
                _closeButton.Location = new Point(_titleBarPanel.Width - 35, 5);
                _maximizeButton.Location = new Point(_titleBarPanel.Width - 70, 5);
                _minimizeButton.Location = new Point(_titleBarPanel.Width - 105, 5);
            }
        }

        private Button CreateWindowButton(string text, Color hoverColor)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(30, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = hoverColor;

            return button;
        }

        private void ApplyThemeToControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control == _titleBarPanel) continue; // Älä muuta title baria

                // Apply theme eri control tyypeille
                switch (control)
                {
                    case TabControl tabControl:
                        tabControl.BackColor = Color.FromArgb(40, 40, 40);
                        tabControl.ForeColor = Color.White;
                        break;
                    case Panel panel:
                        panel.BackColor = Color.FromArgb(40, 40, 40);
                        panel.ForeColor = Color.White;
                        break;
                    case GroupBox groupBox:
                        groupBox.BackColor = Color.FromArgb(40, 40, 40);
                        groupBox.ForeColor = Color.White;
                        break;
                    case Label label:
                        if (label.BackColor != Color.Transparent)
                            label.BackColor = Color.FromArgb(40, 40, 40);
                        label.ForeColor = Color.White;
                        break;
                    case TextBox textBox:
                        textBox.BackColor = Color.FromArgb(60, 60, 60);
                        textBox.ForeColor = Color.White;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case ComboBox comboBox:
                        comboBox.BackColor = Color.FromArgb(60, 60, 60);
                        comboBox.ForeColor = Color.White;
                        comboBox.FlatStyle = FlatStyle.Flat;
                        break;
                    case ListBox listBox:
                        listBox.BackColor = Color.FromArgb(60, 60, 60);
                        listBox.ForeColor = Color.White;
                        listBox.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case Button button:
                        if (!IsWindowButton(button))
                            ApplyButtonTheme(button);
                        break;
                    case CheckBox checkBox:
                        checkBox.BackColor = Color.FromArgb(40, 40, 40);
                        checkBox.ForeColor = Color.White;
                        break;
                    case RadioButton radioButton:
                        radioButton.BackColor = Color.FromArgb(40, 40, 40);
                        radioButton.ForeColor = Color.White;
                        break;
                }

                // Rekursiivisesti child controllit
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control);
                }
            }
        }

        private bool IsWindowButton(Button button)
        {
            return button == _closeButton || button == _maximizeButton || button == _minimizeButton;
        }

        private void ApplyButtonTheme(Button button)
        {
            button.BackColor = _accentColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(_accentColor, 0.1f);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(_accentColor, 0.1f);
            button.Cursor = Cursors.Hand;
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_isResizing)
            {
                _isDragging = true;
                _lastCursor = Cursor.Position;
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.Button == MouseButtons.Left && !_isResizing)
            {
                Point currentPos = Cursor.Position;
                _parentForm.Location = new Point(
                    _parentForm.Location.X + (currentPos.X - _lastCursor.X),
                    _parentForm.Location.Y + (currentPos.Y - _lastCursor.Y));
                _lastCursor = currentPos;
            }
        }

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            if (_parentForm.WindowState == FormWindowState.Maximized)
            {
                _parentForm.WindowState = FormWindowState.Normal;
                _maximizeButton.Text = "□";
            }
            else
            {
                _parentForm.WindowState = FormWindowState.Maximized;
                _maximizeButton.Text = "❐";
            }
        }

        private void ParentForm_Paint(object sender, PaintEventArgs e)
        {
            // Piirrä moderni border
            using (var borderPen = new Pen(_accentColor, 2))
            {
                var rect = new Rectangle(0, 0, _parentForm.Width - 1, _parentForm.Height - 1);
                e.Graphics.DrawRectangle(borderPen, rect);
            }
        }

        #region Resize Functionality
        private void ParentForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_parentForm.WindowState == FormWindowState.Maximized)
            {
                _parentForm.Cursor = Cursors.Default;
                return;
            }

            if (_isResizing)
            {
                HandleResize(e.Location);
                return;
            }

            // Tarkista resize suunta ja aseta cursor
            var direction = GetResizeDirection(e.Location);
            SetResizeCursor(direction);
        }

        private void ParentForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (_parentForm.WindowState == FormWindowState.Maximized || e.Button != MouseButtons.Left)
                return;

            _resizeDirection = GetResizeDirection(e.Location);
            if (_resizeDirection != ResizeDirection.None)
            {
                _isResizing = true;
                _lastCursor = Cursor.Position;
            }
        }

        private void ParentForm_MouseUp(object sender, MouseEventArgs e)
        {
            _isResizing = false;
            _resizeDirection = ResizeDirection.None;
        }

        private ResizeDirection GetResizeDirection(Point location)
        {
            var form = _parentForm;
            var titleBarHeight = _titleBarPanel?.Height ?? 35;

            // Tarkista kulmat ensin (isompi prioriteetti)
            if (location.X <= RESIZE_BORDER_WIDTH && location.Y <= RESIZE_BORDER_WIDTH)
                return ResizeDirection.TopLeft;
            if (location.X >= form.Width - RESIZE_BORDER_WIDTH && location.Y <= RESIZE_BORDER_WIDTH)
                return ResizeDirection.TopRight;
            if (location.X <= RESIZE_BORDER_WIDTH && location.Y >= form.Height - RESIZE_BORDER_WIDTH)
                return ResizeDirection.BottomLeft;
            if (location.X >= form.Width - RESIZE_BORDER_WIDTH && location.Y >= form.Height - RESIZE_BORDER_WIDTH)
                return ResizeDirection.BottomRight;

            // Tarkista reunat
            if (location.X <= RESIZE_BORDER_WIDTH)
                return ResizeDirection.Left;
            if (location.X >= form.Width - RESIZE_BORDER_WIDTH)
                return ResizeDirection.Right;
            if (location.Y <= RESIZE_BORDER_WIDTH)
                return ResizeDirection.Top;
            if (location.Y >= form.Height - RESIZE_BORDER_WIDTH)
                return ResizeDirection.Bottom;

            return ResizeDirection.None;
        }

        private void SetResizeCursor(ResizeDirection direction)
        {
            switch (direction)
            {
                case ResizeDirection.Left:
                case ResizeDirection.Right:
                    _parentForm.Cursor = Cursors.SizeWE;
                    break;
                case ResizeDirection.Top:
                case ResizeDirection.Bottom:
                    _parentForm.Cursor = Cursors.SizeNS;
                    break;
                case ResizeDirection.TopLeft:
                case ResizeDirection.BottomRight:
                    _parentForm.Cursor = Cursors.SizeNWSE;
                    break;
                case ResizeDirection.TopRight:
                case ResizeDirection.BottomLeft:
                    _parentForm.Cursor = Cursors.SizeNESW;
                    break;
                default:
                    _parentForm.Cursor = Cursors.Default;
                    break;
            }
        }

        private void HandleResize(Point currentLocation)
        {
            var currentCursor = Cursor.Position;
            var deltaX = currentCursor.X - _lastCursor.X;
            var deltaY = currentCursor.Y - _lastCursor.Y;

            var newBounds = _parentForm.Bounds;
            var minWidth = _parentForm.MinimumSize.Width == 0 ? 300 : _parentForm.MinimumSize.Width;
            var minHeight = _parentForm.MinimumSize.Height == 0 ? 200 : _parentForm.MinimumSize.Height;

            switch (_resizeDirection)
            {
                case ResizeDirection.Left:
                    newBounds.X += deltaX;
                    newBounds.Width -= deltaX;
                    if (newBounds.Width < minWidth)
                    {
                        newBounds.X = newBounds.Right - minWidth;
                        newBounds.Width = minWidth;
                    }
                    break;

                case ResizeDirection.Right:
                    newBounds.Width += deltaX;
                    if (newBounds.Width < minWidth)
                        newBounds.Width = minWidth;
                    break;

                case ResizeDirection.Top:
                    newBounds.Y += deltaY;
                    newBounds.Height -= deltaY;
                    if (newBounds.Height < minHeight)
                    {
                        newBounds.Y = newBounds.Bottom - minHeight;
                        newBounds.Height = minHeight;
                    }
                    break;

                case ResizeDirection.Bottom:
                    newBounds.Height += deltaY;
                    if (newBounds.Height < minHeight)
                        newBounds.Height = minHeight;
                    break;

                case ResizeDirection.TopLeft:
                    newBounds.X += deltaX;
                    newBounds.Y += deltaY;
                    newBounds.Width -= deltaX;
                    newBounds.Height -= deltaY;
                    if (newBounds.Width < minWidth)
                    {
                        newBounds.X = newBounds.Right - minWidth;
                        newBounds.Width = minWidth;
                    }
                    if (newBounds.Height < minHeight)
                    {
                        newBounds.Y = newBounds.Bottom - minHeight;
                        newBounds.Height = minHeight;
                    }
                    break;

                case ResizeDirection.TopRight:
                    newBounds.Y += deltaY;
                    newBounds.Width += deltaX;
                    newBounds.Height -= deltaY;
                    if (newBounds.Width < minWidth)
                        newBounds.Width = minWidth;
                    if (newBounds.Height < minHeight)
                    {
                        newBounds.Y = newBounds.Bottom - minHeight;
                        newBounds.Height = minHeight;
                    }
                    break;

                case ResizeDirection.BottomLeft:
                    newBounds.X += deltaX;
                    newBounds.Width -= deltaX;
                    newBounds.Height += deltaY;
                    if (newBounds.Width < minWidth)
                    {
                        newBounds.X = newBounds.Right - minWidth;
                        newBounds.Width = minWidth;
                    }
                    if (newBounds.Height < minHeight)
                        newBounds.Height = minHeight;
                    break;

                case ResizeDirection.BottomRight:
                    newBounds.Width += deltaX;
                    newBounds.Height += deltaY;
                    if (newBounds.Width < minWidth)
                        newBounds.Width = minWidth;
                    if (newBounds.Height < minHeight)
                        newBounds.Height = minHeight;
                    break;
            }

            _parentForm.Bounds = newBounds;
            _lastCursor = currentCursor;
        }
        #endregion

        #region Layout Management
        private void InitializeLayout()
        {
            if (_parentForm == null || _layoutInitialized) return;

            _originalFormSize = _parentForm.ClientSize;
            _controlLayouts.Clear();

            // Tallenna kaikkien kontrollien alkuperäiset sijainnit ja koot
            SaveControlLayouts(_parentForm);
            _layoutInitialized = true;
        }

        private void SaveControlLayouts(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control == _titleBarPanel) continue; // Älä tallenna title baria

                // Tallenna suhteelliset sijainnit ja koot
                var layout = new ControlLayout
                {
                    XRatio = (float)control.Left / _originalFormSize.Width,
                    YRatio = (float)control.Top / _originalFormSize.Height,
                    WidthRatio = (float)control.Width / _originalFormSize.Width,
                    HeightRatio = (float)control.Height / _originalFormSize.Height,
                    OriginalAnchor = control.Anchor,
                    OriginalDock = control.Dock
                };

                _controlLayouts[control] = layout;

                // Rekursiivisesti child kontrollit
                if (control.HasChildren && !(control is TextBox) && !(control is ComboBox))
                {
                    SaveControlLayouts(control);
                }
            }
        }

        private void ParentForm_Resize(object sender, EventArgs e)
        {
            if (!_layoutInitialized || _parentForm == null) return;

            // Päivitä kontrollit uuden koon mukaan
            ApplyLayoutToControls(_parentForm);
        }

        private void ApplyLayoutToControls(Control parent)
        {
            var currentSize = _parentForm.ClientSize;

            foreach (Control control in parent.Controls)
            {
                if (control == _titleBarPanel) continue;

                if (_controlLayouts.TryGetValue(control, out var layout))
                {
                    // Laske uudet sijainnit ja koot suhteellisesti
                    var newX = (int)(layout.XRatio * currentSize.Width);
                    var newY = (int)(layout.YRatio * currentSize.Height);
                    var newWidth = (int)(layout.WidthRatio * currentSize.Width);
                    var newHeight = (int)(layout.HeightRatio * currentSize.Height);

                    // Varmista että kontrollilla on minimikoko
                    newWidth = Math.Max(newWidth, 50);
                    newHeight = Math.Max(newHeight, 20);

                    // Varmista että kontrolli pysyy näkyvissä
                    newX = Math.Max(0, Math.Min(newX, currentSize.Width - newWidth));
                    newY = Math.Max(_titleBarPanel?.Height ?? 35, Math.Min(newY, currentSize.Height - newHeight));

                    try
                    {
                        control.SuspendLayout();

                        // Päivitä sijainti ja koko
                        control.Location = new Point(newX, newY);
                        control.Size = new Size(newWidth, newHeight);

                        control.ResumeLayout();
                    }
                    catch
                    {
                        // Jos kontrollin päivitys epäonnistuu, jätä ennalleen
                    }

                    // Rekursiivisesti child kontrollit
                    if (control.HasChildren && !(control is TextBox) && !(control is ComboBox))
                    {
                        ApplyLayoutToControls(control);
                    }
                }
            }

            // Force form to update display
            _parentForm.Invalidate();
        }
        #endregion

        private void RemoveTheme()
        {
            if (_parentForm != null)
            {
                // Poista event handlerit
                _parentForm.Paint -= ParentForm_Paint;
                _parentForm.Resize -= UpdateButtonPositions;
                _parentForm.Resize -= ParentForm_Resize;
                _parentForm.MouseMove -= ParentForm_MouseMove;
                _parentForm.MouseDown -= ParentForm_MouseDown;
                _parentForm.MouseUp -= ParentForm_MouseUp;

                // Poista title bar
                if (_titleBarPanel != null)
                {
                    _parentForm.Controls.Remove(_titleBarPanel);
                    _titleBarPanel?.Dispose();
                    _titleBarPanel = null;
                }

                // Nollaa button viittaukset
                _closeButton = null;
                _maximizeButton = null;
                _minimizeButton = null;
                _titleLabel = null;

                // Nollaa layout management
                _controlLayouts.Clear();
                _layoutInitialized = false;

                // Palauta alkuperäinen border (vain jos ei design mode)
                if (!this.DesignMode)
                {
                    _parentForm.FormBorderStyle = FormBorderStyle.Sizable;
                    _parentForm.Padding = new Padding(0);
                    _parentForm.Cursor = Cursors.Default;
                }

                _themeApplied = false;
                _isResizing = false;
                _isDragging = false;
                _resizeDirection = ResizeDirection.None;
            }
        }
    }
    // Custom TabControl (tumma teema)
    [ToolboxItem(true)]
    public class LoyalTabControl : TabControl
    {
        private Color _tabColor = Color.FromArgb(39, 174, 96);
        private SubTabCollection _subTabs;
        private int _subTabHeight = 28; // Pienempi korkeus

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TabHighlightColor
        {
            get { return _tabColor; }
            set
            {
                _tabColor = value;
                Invalidate();
            }
        }

        [Category("SubTabs")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.CollectionEditor", "System.Drawing.Design.UITypeEditor")]
        public SubTabCollection SubTabs
        {
            get
            {
                if (_subTabs == null)
                {
                    _subTabs = new SubTabCollection(this);
                }
                return _subTabs;
            }
        }

        // Designer serialization support
        public bool ShouldSerializeSubTabs()
        {
            return _subTabs?.Count > 0;
        }

        [Category("SubTabs")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(28)]
        public int SubTabHeight
        {
            get { return _subTabHeight; }
            set
            {
                _subTabHeight = Math.Max(20, Math.Min(50, value)); // 20-50px välillä
                UpdateSubTabs();
                Invalidate();
            }
        }

        // Legacy property names for backward compatibility
        [Browsable(false)]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Obsolete("Use SubTabHeight instead")]
        [DefaultValue(28)]
        public int ButtonRowHeight
        {
            get { return SubTabHeight; }
            set { SubTabHeight = value; }
        }

        [Browsable(false)]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Obsolete("Use SubTabs.Count > 0 instead")]
        [DefaultValue(false)]
        public bool ShowButtonRow
        {
            get { return _subTabs?.Count > 0; }
            set
            {
                if (value && (_subTabs?.Count == 0 || _subTabs == null))
                {
                    // Add default subtabs for compatibility
                    SubTabs.Add(new SubTab("Settings"));
                    SubTabs.Add(new SubTab("Tools"));
                }
                else if (!value)
                {
                    SubTabs.Clear();
                }
            }
        }

        public LoyalTabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            DrawMode = TabDrawMode.OwnerDrawFixed;
            ItemSize = new Size(100, 35);
            SizeMode = TabSizeMode.Fixed;
            Alignment = TabAlignment.Top;
            BackColor = Color.FromArgb(40, 40, 40);
            Multiline = false; // Pakottaa yhden rivin, poistaa sisäänrakennetut scroll napit

            // Piilota oletusarvoinen scroll-toiminnallisuus
            this.SetStyle(ControlStyles.UserPaint, true);

            // Alusta SubTabs collection
            _subTabs = new SubTabCollection(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(Color.FromArgb(31, 31, 31));
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < TabPages.Count; i++)
            {
                Rectangle tabRect = GetTabRect(i);
                bool isSelected = (i == SelectedIndex);

                // Täytä tausta modernilla gradientilla
                Color fillColor = isSelected ? _tabColor : Color.FromArgb(45, 45, 45);

                using (SolidBrush bgBrush = new SolidBrush(fillColor))
                {
                    e.Graphics.FillRectangle(bgBrush, tabRect);
                }

                // Piirrä hover effect
                if (!isSelected && tabRect.Contains(PointToClient(Cursor.Position)))
                {
                    using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(30, 255, 255, 255)))
                    {
                        e.Graphics.FillRectangle(hoverBrush, tabRect);
                    }
                }

                // Piirrä modernit reunat
                using (Pen borderPen = new Pen(Color.FromArgb(70, 70, 70), 1))
                {
                    e.Graphics.DrawRectangle(borderPen, tabRect);
                }

                // Piirrä aktiivisen tabin korostus
                if (isSelected)
                {
                    using (Pen accentPen = new Pen(_tabColor, 3))
                    {
                        e.Graphics.DrawLine(accentPen, tabRect.X, tabRect.Bottom - 1, tabRect.Right, tabRect.Bottom - 1);
                    }
                }

                // Piirrä teksti
                using (Brush textColor = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(TabPages[i].Text, new Font("Segoe UI", 9, FontStyle.Bold), textColor, tabRect, sf);
                }
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            Appearance = TabAppearance.FlatButtons;
            Multiline = false;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.BackColor = Color.FromArgb(40, 40, 40);
        }

        internal void UpdateSubTabs()
        {
            // Null check for Controls
            if (Controls == null)
                return;

            // Poista vanhat SubTab buttonit
            var subTabButtons = Controls.OfType<Button>().Where(b => b.Tag?.ToString() == "SubTab").ToList();
            foreach (var button in subTabButtons)
            {
                Controls.Remove(button);
                button.Dispose();
            }

            // Null check for _subTabs
            if (_subTabs != null && _subTabs.Count > 0)
            {
                PositionSubTabs();
            }
        }

        private void PositionSubTabs()
        {
            if (_subTabs == null) return;

            int x = 5;
            int y = ItemSize.Height + 2; // Tabien alapuolelle

            foreach (SubTab subTab in _subTabs)
            {
                if (subTab.Button == null)
                {
                    // Luo button SubTabille
                    subTab.Button = new Button
                    {
                        Text = subTab.Text,
                        Size = new Size(80, _subTabHeight),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(50, 50, 50),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 8, FontStyle.Regular),
                        Cursor = Cursors.Hand,
                        Tag = "SubTab" // Mark that this is SubTab
                    };

                    subTab.Button.FlatAppearance.BorderSize = 0;
                    subTab.Button.FlatAppearance.MouseOverBackColor = _tabColor;
                    subTab.Button.Click += (s, e) => OnSubTabClick(subTab);

                    Controls.Add(subTab.Button);
                }

                subTab.Button.Location = new Point(x, y);
                x += subTab.Button.Width + 3; // 3px väli
            }
        }

        // SubTab click event
        protected virtual void OnSubTabClick(SubTab subTab)
        {
            // Override tätä tai käytä SubTabClicked eventtiä
            SubTabClicked?.Invoke(this, new SubTabEventArgs(subTab));
        }

        // SubTab clicked event
        public event EventHandler<SubTabEventArgs> SubTabClicked;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionSubTabs();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_subTabs != null)
                {
                    foreach (SubTab subTab in _subTabs)
                    {
                        subTab.Button?.Dispose();
                    }
                    _subTabs.Clear();
                }
            }
            base.Dispose(disposing);
        }
    }

    // SubTab luokka
    public class SubTab
    {
        public string Text { get; set; } = "SubTab";
        public string Name { get; set; } = "";
        public object Tag { get; set; }
        internal Button Button { get; set; }

        public SubTab() { }
        public SubTab(string text) { Text = text; }
    }

    // SubTab Collection
    public class SubTabCollection : CollectionBase
    {
        private LoyalTabControl _owner;

        internal SubTabCollection(LoyalTabControl owner)
        {
            _owner = owner;
        }

        public SubTab this[int index]
        {
            get { return (SubTab)List[index]; }
            set { List[index] = value; }
        }

        public int Add(SubTab subTab)
        {
            var result = List.Add(subTab);
            _owner?.UpdateSubTabs();
            return result;
        }

        public void AddRange(SubTab[] subTabs)
        {
            foreach (var subTab in subTabs)
            {
                List.Add(subTab);
            }
            _owner?.UpdateSubTabs();
        }

        public void Remove(SubTab subTab)
        {
            List.Remove(subTab);
            _owner?.UpdateSubTabs();
        }

        public bool Contains(SubTab subTab)
        {
            return List.Contains(subTab);
        }

        public int IndexOf(SubTab subTab)
        {
            return List.IndexOf(subTab);
        }

        public void Insert(int index, SubTab subTab)
        {
            List.Insert(index, subTab);
            _owner?.UpdateSubTabs();
        }

        protected override void OnClear()
        {
            base.OnClear();
            _owner?.UpdateSubTabs();
        }
    }

    // SubTab Event Args
    public class SubTabEventArgs : EventArgs
    {
        public SubTab SubTab { get; }

        public SubTabEventArgs(SubTab subTab)
        {
            SubTab = subTab;
        }
    }

    // Moderni Button custom control
    [ToolboxItem(true)]
    public class ModernButton : Button
    {
        private Color _primaryColor = Color.FromArgb(39, 174, 96);
        private Color _hoverColor = Color.FromArgb(46, 204, 113);
        private Color _pressedColor = Color.FromArgb(34, 153, 84);
        private int _cornerRadius = 8;

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set { _primaryColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverColor
        {
            get { return _hoverColor; }
            set { _hoverColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color PressedColor
        {
            get { return _pressedColor; }
            set { _pressedColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set { _cornerRadius = value; Invalidate(); }
        }

        public ModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            ForeColor = Color.White;
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color currentColor = _primaryColor;
            if (MouseButtons == MouseButtons.Left && ClientRectangle.Contains(PointToClient(Cursor.Position)))
                currentColor = _pressedColor;
            else if (ClientRectangle.Contains(PointToClient(Cursor.Position)))
                currentColor = _hoverColor;

            using (var brush = new SolidBrush(currentColor))
            {
                var rect = new Rectangle(0, 0, Width, Height);
                var path = CreateRoundedRectangle(rect, _cornerRadius);
                e.Graphics.FillPath(brush, path);
            }

            // Piirrä teksti
            var textRect = ClientRectangle;
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), textRect, sf);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius, rect.Y, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius, rect.Y + rect.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }
    }

    // Moderni Panel custom control (kortti-tyyli)
    [ToolboxItem(true)]
    public class ModernCard : Panel
    {
        private Color _cardColor = Color.FromArgb(58, 58, 58);
        private int _cornerRadius = 10;
        private Color _shadowColor = Color.FromArgb(50, 0, 0, 0);

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CardColor
        {
            get { return _cardColor; }
            set { _cardColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set { _cornerRadius = value; Invalidate(); }
        }

        public ModernCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            Padding = new Padding(15);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(2, 2, Width - 4, Height - 4);
            var shadowRect = new Rectangle(4, 4, Width - 4, Height - 4);

            // Piirrä varjo
            using (var shadowBrush = new SolidBrush(_shadowColor))
            {
                var shadowPath = CreateRoundedRectangle(shadowRect, _cornerRadius);
                e.Graphics.FillPath(shadowBrush, shadowPath);
            }

            // Piirrä kortti
            using (var cardBrush = new SolidBrush(_cardColor))
            {
                var cardPath = CreateRoundedRectangle(rect, _cornerRadius);
                e.Graphics.FillPath(cardBrush, cardPath);
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius, rect.Y, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius, rect.Y + rect.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();
            return path;
        }
    }

    // Moderni TextBox
    [ToolboxItem(true)]
    public class ModernTextBox : TextBox
    {
        private Color _borderColor = Color.FromArgb(70, 70, 70);
        private Color _focusColor = Color.FromArgb(39, 174, 96);

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color FocusColor
        {
            get { return _focusColor; }
            set { _focusColor = value; Invalidate(); }
        }

        public ModernTextBox()
        {
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = Color.FromArgb(60, 60, 60);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0xF) // WM_PAINT
            {
                using (Graphics g = CreateGraphics())
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    var borderColor = Focused ? _focusColor : _borderColor;
                    using (var pen = new Pen(borderColor, 2))
                    {
                        g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                    }
                }
            }
        }
    }

    // Moderni CheckBox
    [ToolboxItem(true)]
    public class ModernCheckBox : Control
    {
        private bool _checked = false;
        private Color _checkedColor = Color.FromArgb(39, 174, 96);
        private Color _uncheckedColor = Color.FromArgb(70, 70, 70);
        private Color _hoverColor = Color.FromArgb(85, 85, 85);
        private Color _textColor = Color.White;
        private Color _borderColor = Color.FromArgb(70, 70, 70);
        private Color _borderHoverColor = Color.FromArgb(39, 174, 96);
        private Color _borderFocusColor = Color.FromArgb(46, 204, 113);
        private int _boxSize = 18;
        private bool _isHovering = false;
        private bool _isFocused = false;

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    Invalidate();
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CheckedColor
        {
            get { return _checkedColor; }
            set { _checkedColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color UncheckedColor
        {
            get { return _uncheckedColor; }
            set { _uncheckedColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderHoverColor
        {
            get { return _borderHoverColor; }
            set { _borderHoverColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderFocusColor
        {
            get { return _borderFocusColor; }
            set { _borderFocusColor = value; Invalidate(); }
        }

        public event EventHandler CheckedChanged;

        public ModernCheckBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.Selectable, true);
            Size = new Size(120, 25);
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(40, 40, 40);
            Cursor = Cursors.Hand;
            TabStop = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var boxRect = new Rectangle(2, (Height - _boxSize) / 2, _boxSize, _boxSize);
            Color boxColor = _checked ? _checkedColor : (_isHovering ? _hoverColor : _uncheckedColor);
            Color currentBorderColor = _isFocused ? _borderFocusColor : (_isHovering ? _borderHoverColor : _borderColor);

            // Piirrä checkbox tausta
            using (var brush = new SolidBrush(boxColor))
                e.Graphics.FillRectangle(brush, boxRect);

            // Piirrä border
            using (var borderPen = new Pen(currentBorderColor, 2))
                e.Graphics.DrawRectangle(borderPen, boxRect);

            // Piirrä checkmark
            if (_checked)
            {
                using (var pen = new Pen(Color.White, 2))
                {
                    e.Graphics.DrawLine(pen, boxRect.X + 4, boxRect.Y + _boxSize / 2,
                                       boxRect.X + _boxSize / 2, boxRect.Y + _boxSize - 4);
                    e.Graphics.DrawLine(pen, boxRect.X + _boxSize / 2, boxRect.Y + _boxSize - 4,
                                       boxRect.X + _boxSize - 3, boxRect.Y + 3);
                }
            }

            // Piirrä teksti
            if (!string.IsNullOrEmpty(Text))
            {
                var textRect = new Rectangle(boxRect.Right + 8, 0, Width - boxRect.Right - 8, Height);
                using (var textBrush = new SolidBrush(_textColor))
                {
                    var sf = new StringFormat { LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString(Text, Font, textBrush, textRect, sf);
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            base.OnClick(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovering = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovering = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            _isFocused = true;
            Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            _isFocused = false;
            Invalidate();
            base.OnLostFocus(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData == Keys.Space || keyData == Keys.Enter || base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                Checked = !Checked;
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }

    // Moderni Toggle Switch
    [ToolboxItem(true)]
    public class ModernToggleSwitch : Control
    {
        private bool _checked = false;
        private Color _onColor = Color.FromArgb(39, 174, 96);
        private Color _offColor = Color.FromArgb(70, 70, 70);
        private Color _thumbOnColor = Color.White;
        private Color _thumbOffColor = Color.FromArgb(220, 220, 220);
        private Color _borderColor = Color.FromArgb(85, 85, 85);
        private Color _hoverColor = Color.FromArgb(100, 100, 100);
        private Color _shadowColor = Color.FromArgb(30, 0, 0, 0);
        private int _animationTimer = 0;
        private int _cornerRadius = 12;
        private int _borderWidth = 1;
        private bool _isHovering = false;
        private ToggleStyle _style = ToggleStyle.Modern;
        private bool _useGradient = true;
        private bool _useShadow = true;
        private int _thumbPadding = 2;
        private System.Windows.Forms.Timer _timer;

        public enum ToggleStyle
        {
            Modern,      // Alkuperäinen moderni tyyli
            Minimal,     // Minimalistinen tyyli
            iOS,         // iOS-tyylinen
            Material,    // Material Design
            Retro,       // Retro-tyyli
            Neon         // Neon-efekti
        }

        [Category("Appearance")]
        [Description("Checked state of the toggle switch")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    StartAnimation();
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Category("Appearance")]
        [Description("Color when toggle is ON")]
        [DefaultValue(typeof(Color), "39, 174, 96")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color OnColor
        {
            get { return _onColor; }
            set { _onColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color when toggle is OFF")]
        [DefaultValue(typeof(Color), "70, 70, 70")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color OffColor
        {
            get { return _offColor; }
            set { _offColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Thumb color when toggle is ON")]
        [DefaultValue(typeof(Color), "White")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ThumbOnColor
        {
            get { return _thumbOnColor; }
            set { _thumbOnColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Thumb color when toggle is OFF")]
        [DefaultValue(typeof(Color), "220, 220, 220")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ThumbOffColor
        {
            get { return _thumbOffColor; }
            set { _thumbOffColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Border color of the toggle")]
        [DefaultValue(typeof(Color), "85, 85, 85")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Width of the border (0 = no border)")]
        [DefaultValue(1)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = Math.Max(0, Math.Min(5, value)); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color when hovering over the toggle")]
        [DefaultValue(typeof(Color), "100, 100, 100")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverColor
        {
            get { return _hoverColor; }
            set { _hoverColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Shadow color for depth effect")]
        [DefaultValue(typeof(Color), "30, 0, 0, 0")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ShadowColor
        {
            get { return _shadowColor; }
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("Layout")]
        [Description("Corner radius for rounded edges")]
        [DefaultValue(12)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set { _cornerRadius = Math.Max(1, value); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Visual style of the toggle switch")]
        [DefaultValue(ToggleStyle.Modern)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ToggleStyle Style
        {
            get { return _style; }
            set { _style = value; ApplyStyle(); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Use gradient fill for modern look")]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseGradient
        {
            get { return _useGradient; }
            set { _useGradient = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Add shadow for depth effect")]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseShadow
        {
            get { return _useShadow; }
            set { _useShadow = value; Invalidate(); }
        }

        [Category("Layout")]
        [Description("Padding around the thumb inside the track")]
        [DefaultValue(2)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ThumbPadding
        {
            get { return _thumbPadding; }
            set { _thumbPadding = Math.Max(1, Math.Min(8, value)); Invalidate(); }
        }

        public event EventHandler CheckedChanged;

        public ModernToggleSwitch()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(50, 25);
            Cursor = Cursors.Hand;
            TabStop = true;

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 10;
            _timer.Tick += Timer_Tick;
            
            ApplyStyle(); // Aseta oletustyyli
        }

        private void ApplyStyle()
        {
            switch (_style)
            {
                case ToggleStyle.Modern:
                    _onColor = Color.FromArgb(39, 174, 96);
                    _offColor = Color.FromArgb(70, 70, 70);
                    _thumbOnColor = Color.White;
                    _thumbOffColor = Color.FromArgb(220, 220, 220);
                    _borderColor = Color.FromArgb(85, 85, 85);
                    _cornerRadius = 12;
                    _useGradient = true;
                    _useShadow = true;
                    _thumbPadding = 2;
                    _borderWidth = 1;
                    break;

                case ToggleStyle.Minimal:
                    _onColor = Color.FromArgb(100, 100, 100);
                    _offColor = Color.FromArgb(200, 200, 200);
                    _thumbOnColor = Color.White;
                    _thumbOffColor = Color.FromArgb(120, 120, 120);
                    _borderColor = Color.FromArgb(150, 150, 150);
                    _cornerRadius = 2;
                    _useGradient = false;
                    _useShadow = false;
                    _thumbPadding = 3;
                    _borderWidth = 1;
                    break;

                case ToggleStyle.iOS:
                    _onColor = Color.FromArgb(52, 199, 89);
                    _offColor = Color.FromArgb(209, 209, 214);
                    _thumbOnColor = Color.White;
                    _thumbOffColor = Color.White;
                    _borderColor = Color.Transparent;
                    _cornerRadius = Height / 2;
                    _useGradient = true;
                    _useShadow = true;
                    _thumbPadding = 2;
                    _borderWidth = 0;
                    break;

                case ToggleStyle.Material:
                    _onColor = Color.FromArgb(33, 150, 243);
                    _offColor = Color.FromArgb(158, 158, 158);
                    _thumbOnColor = Color.FromArgb(33, 150, 243);
                    _thumbOffColor = Color.FromArgb(245, 245, 245);
                    _borderColor = Color.FromArgb(224, 224, 224);
                    _cornerRadius = Height / 2;
                    _useGradient = false;
                    _useShadow = true;
                    _thumbPadding = 3;
                    _borderWidth = 1;
                    break;

                case ToggleStyle.Retro:
                    _onColor = Color.FromArgb(255, 193, 7);
                    _offColor = Color.FromArgb(108, 117, 125);
                    _thumbOnColor = Color.FromArgb(255, 235, 59);
                    _thumbOffColor = Color.FromArgb(173, 181, 189);
                    _borderColor = Color.FromArgb(52, 58, 64);
                    _cornerRadius = 4;
                    _useGradient = true;
                    _useShadow = false;
                    _thumbPadding = 4;
                    _borderWidth = 2;
                    break;

                case ToggleStyle.Neon:
                    _onColor = Color.FromArgb(0, 255, 255);
                    _offColor = Color.FromArgb(50, 50, 50);
                    _thumbOnColor = Color.FromArgb(255, 0, 255);
                    _thumbOffColor = Color.FromArgb(100, 100, 100);
                    _borderColor = Color.FromArgb(0, 255, 255);
                    _cornerRadius = 8;
                    _useGradient = true;
                    _useShadow = true;
                    _thumbPadding = 2;
                    _borderWidth = 2;
                    break;
            }
        }

        private void StartAnimation()
        {
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_checked)
            {
                if (_animationTimer < 10)
                    _animationTimer++;
                else
                    _timer.Stop();
            }
            else
            {
                if (_animationTimer > 0)
                    _animationTimer--;
                else
                    _timer.Stop();
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var trackRect = new Rectangle(0, 0, Width, Height);
            var thumbSize = Height - (_thumbPadding * 2);
            var thumbX = _thumbPadding + (_animationTimer * (Width - thumbSize - (_thumbPadding * 2))) / 10;
            var thumbRect = new Rectangle(thumbX, _thumbPadding, thumbSize, thumbSize);

            // Interpoloi värit animaation mukaan
            var progress = (float)_animationTimer / 10f;
            var currentTrackColor = InterpolateColor(_offColor, _onColor, progress);
            var currentThumbColor = InterpolateColor(_thumbOffColor, _thumbOnColor, progress);

            // Hover effect
            if (_isHovering && !_checked)
            {
                currentTrackColor = InterpolateColor(currentTrackColor, _hoverColor, 0.3f);
            }

            // Piirrä track
            if (_useGradient && _style != ToggleStyle.Minimal)
            {
                var gradientEndColor = GetDarkerColor(currentTrackColor, 0.15f);
                using (var gradientBrush = new LinearGradientBrush(trackRect, currentTrackColor, gradientEndColor, LinearGradientMode.Vertical))
                {
                    var trackPath = CreateRoundedRectangle(trackRect, _cornerRadius);
                    e.Graphics.FillPath(gradientBrush, trackPath);
                }
            }
            else
            {
                using (var trackBrush = new SolidBrush(currentTrackColor))
                {
                    var trackPath = CreateRoundedRectangle(trackRect, _cornerRadius);
                    e.Graphics.FillPath(trackBrush, trackPath);
                }
            }

            // Piirrä border
            if (_borderWidth > 0 && _borderColor != Color.Transparent)
            {
                using (var borderPen = new Pen(_borderColor, _borderWidth))
                {
                    var borderPath = CreateRoundedRectangle(trackRect, _cornerRadius);
                    e.Graphics.DrawPath(borderPen, borderPath);
                }
            }

            // Neon-efekti
            if (_style == ToggleStyle.Neon && _checked)
            {
                using (var glowPen = new Pen(Color.FromArgb(100, _onColor), 3))
                {
                    var glowPath = CreateRoundedRectangle(new Rectangle(-1, -1, Width + 2, Height + 2), _cornerRadius + 1);
                    e.Graphics.DrawPath(glowPen, glowPath);
                }
            }

            // Piirrä thumb shadow
            if (_useShadow)
            {
                using (var shadowBrush = new SolidBrush(_shadowColor))
                {
                    var shadowRect = new Rectangle(thumbRect.X + 1, thumbRect.Y + 1, thumbRect.Width, thumbRect.Height);
                    if (_style == ToggleStyle.Material)
                    {
                        var shadowPath = CreateRoundedRectangle(shadowRect, thumbRect.Height / 2);
                        e.Graphics.FillPath(shadowBrush, shadowPath);
                    }
                    else
                    {
                        e.Graphics.FillEllipse(shadowBrush, shadowRect);
                    }
                }
            }

            // Piirrä thumb
            using (var thumbBrush = new SolidBrush(currentThumbColor))
            {
                if (_style == ToggleStyle.Material)
                {
                    // Material Design: pyöristetty neliö
                    var thumbPath = CreateRoundedRectangle(thumbRect, thumbRect.Height / 2);
                    e.Graphics.FillPath(thumbBrush, thumbPath);
                }
                else if (_style == ToggleStyle.Minimal)
                {
                    // Minimal: pieni neliö
                    var thumbPath = CreateRoundedRectangle(thumbRect, 2);
                    e.Graphics.FillPath(thumbBrush, thumbPath);
                }
                else
                {
                    // Muut: ympyrä
                    e.Graphics.FillEllipse(thumbBrush, thumbRect);
                }
            }

            // Thumb border joillekin tyyleille
            if (_style == ToggleStyle.iOS || _style == ToggleStyle.Material)
            {
                using (var thumbBorderPen = new Pen(Color.FromArgb(200, 200, 200), 1))
                {
                    if (_style == ToggleStyle.Material)
                    {
                        var thumbPath = CreateRoundedRectangle(thumbRect, thumbRect.Height / 2);
                        e.Graphics.DrawPath(thumbBorderPen, thumbPath);
                    }
                    else
                    {
                        e.Graphics.DrawEllipse(thumbBorderPen, thumbRect);
                    }
                }
            }

            // Focus rectangle
            if (Focused)
            {
                using (var focusPen = new Pen(Color.FromArgb(100, _onColor), 2))
                {
                    focusPen.DashStyle = DashStyle.Dash;
                    var focusRect = new Rectangle(trackRect.X - 2, trackRect.Y - 2, trackRect.Width + 4, trackRect.Height + 4);
                    e.Graphics.DrawRectangle(focusPen, focusRect);
                }
            }
        }

        private Color InterpolateColor(Color color1, Color color2, float progress)
        {
            var r = (int)(color1.R + (color2.R - color1.R) * progress);
            var g = (int)(color1.G + (color2.G - color1.G) * progress);
            var b = (int)(color1.B + (color2.B - color1.B) * progress);
            return Color.FromArgb(r, g, b);
        }

        private Color GetDarkerColor(Color color, float percentage)
        {
            float red = color.R * (1 - percentage);
            float green = color.G * (1 - percentage);
            float blue = color.B * (1 - percentage);
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            if (cornerRadius <= 0)
            {
                path.AddRectangle(rect);
                path.CloseAllFigures();
                return path;
            }

            cornerRadius = Math.Min(cornerRadius, Math.Min(rect.Width / 2, rect.Height / 2));
            
            path.AddArc(rect.X, rect.Y, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius, rect.Y, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius, rect.Y + rect.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            base.OnClick(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovering = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovering = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                Checked = !Checked;
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _timer?.Dispose();
            base.Dispose(disposing);
        }

        // Esimääritetyt tyylit metodien kautta
        public void SetModernStyle() => Style = ToggleStyle.Modern;
        public void SetMinimalStyle() => Style = ToggleStyle.Minimal;
        public void SetiOSStyle() => Style = ToggleStyle.iOS;
        public void SetMaterialStyle() => Style = ToggleStyle.Material;
        public void SetRetroStyle() => Style = ToggleStyle.Retro;
        public void SetNeonStyle() => Style = ToggleStyle.Neon;

        // ShouldSerialize methods for designer
        public bool ShouldSerializeOnColor() => _onColor != Color.FromArgb(39, 174, 96);
        public void ResetOnColor() => OnColor = Color.FromArgb(39, 174, 96);

        public bool ShouldSerializeOffColor() => _offColor != Color.FromArgb(70, 70, 70);
        public void ResetOffColor() => OffColor = Color.FromArgb(70, 70, 70);

        public bool ShouldSerializeThumbOnColor() => _thumbOnColor != Color.White;
        public void ResetThumbOnColor() => ThumbOnColor = Color.White;

        public bool ShouldSerializeThumbOffColor() => _thumbOffColor != Color.FromArgb(220, 220, 220);
        public void ResetThumbOffColor() => ThumbOffColor = Color.FromArgb(220, 220, 220);

        public bool ShouldSerializeBorderColor() => _borderColor != Color.FromArgb(85, 85, 85);
        public void ResetBorderColor() => BorderColor = Color.FromArgb(85, 85, 85);

        public bool ShouldSerializeHoverColor() => _hoverColor != Color.FromArgb(100, 100, 100);
        public void ResetHoverColor() => HoverColor = Color.FromArgb(100, 100, 100);

        public bool ShouldSerializeShadowColor() => _shadowColor != Color.FromArgb(30, 0, 0, 0);
        public void ResetShadowColor() => ShadowColor = Color.FromArgb(30, 0, 0, 0);
    }
}