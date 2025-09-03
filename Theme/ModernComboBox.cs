using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OffCrypt
{
    /// <summary>
    /// Modern ComboBox with customizable border color and dark theme support
    /// </summary>
    [ToolboxItem(true)]
    [Description("Modern ComboBox with dark theme and customizable border")]
    public class ModernComboBox : ComboBox
    {
        private Color _borderColor = Color.FromArgb(39, 174, 96);
        private Color _focusColor = Color.FromArgb(46, 204, 113);
        private Color _hoverColor = Color.FromArgb(52, 52, 55);
        private int _borderWidth = 1;
        private bool _isHovering = false;
        private bool _isFocused = false;

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "39, 174, 96")]
        [Description("Border color of the ComboBox")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "46, 204, 113")]
        [Description("Border color when ComboBox is focused")]
        public Color FocusColor
        {
            get => _focusColor;
            set
            {
                _focusColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "52, 52, 55")]
        [Description("Background color when hovering")]
        public Color HoverColor
        {
            get => _hoverColor;
            set
            {
                _hoverColor = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(1)]
        [Description("Width of the border")]
        public int BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = Math.Max(0, value);
                Invalidate();
            }
        }

        public ModernComboBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            
            // Modern dark theme settings
            BackColor = Color.FromArgb(45, 45, 48);
            ForeColor = Color.White;
            FlatStyle = FlatStyle.Flat;
            DropDownStyle = ComboBoxStyle.DropDownList;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            
            // Disable default border
            FormattingEnabled = true;

            // Owner-draw the dropdown items for consistent dark theme
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = Math.Max(18, Font.Height + 6);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            // Don't call base.OnPaint for custom drawing
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Background
            Color bgColor = (!Enabled) ? Color.FromArgb(40, 40, 40) : (_isHovering ? _hoverColor : BackColor);
            using (SolidBrush bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            // Border
            Color currentBorderColor = !Enabled ? Color.FromArgb(70, 70, 70) : (_isFocused ? _focusColor : _borderColor);
            using (Pen borderPen = new Pen(currentBorderColor, _borderWidth))
            {
                Rectangle borderRect = new Rectangle(0, 0, Width - 1, Height - 1);
                g.DrawRectangle(borderPen, borderRect);
            }

            // Text
            string displayText = GetItemText(SelectedItem);
            if (!string.IsNullOrEmpty(displayText))
            {
                int arrowBox = Math.Max(18, Math.Min(24, Height - 6));
                Rectangle textRect = new Rectangle(8, 0, Math.Max(0, Width - (arrowBox + 12)), Height);
                Color textColor = Enabled ? ForeColor : Color.FromArgb(170, 170, 170);
                TextRenderer.DrawText(g, displayText, Font, textRect, 
                    textColor, TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }

            // Dropdown arrow
            DrawDropdownArrow(g);
        }

        private void DrawDropdownArrow(Graphics g)
        {
            int arrowSize = Math.Max(6, Height / 4);
            int arrowX = Width - Math.Max(18, Math.Min(24, Height - 6));
            int arrowY = (Height - arrowSize) / 2;

            Point[] arrowPoints = {
                new Point(arrowX, arrowY),
                new Point(arrowX + arrowSize, arrowY),
                new Point(arrowX + arrowSize / 2, arrowY + arrowSize / 2)
            };

            Color arrowColor = Enabled ? ForeColor : Color.FromArgb(170, 170, 170);
            using (SolidBrush arrowBrush = new SolidBrush(arrowColor))
            {
                g.FillPolygon(arrowBrush, arrowPoints);
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        // Owner-draw dropdown items to match dark theme
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            e.DrawBackground();

            if (e.Index < 0) return;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color bg = selected ? Color.FromArgb(62, 62, 66) : BackColor;
            using (var b = new SolidBrush(bg)) e.Graphics.FillRectangle(b, e.Bounds);

            string text = GetItemText(Items[e.Index]);
            Color textColor = Enabled ? ForeColor : Color.FromArgb(170, 170, 170);
            TextRenderer.DrawText(e.Graphics, text, Font, e.Bounds, textColor,
                TextFormatFlags.EndEllipsis | TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                e.DrawFocusRectangle();
        }

        /// <summary>
        /// Sets the ComboBox to use OffCrypt's dark theme colors
        /// </summary>
        public void SetDarkTheme()
        {
            BackColor = Color.FromArgb(45, 45, 48);
            ForeColor = Color.White;
            BorderColor = Color.FromArgb(39, 174, 96);
            FocusColor = Color.FromArgb(46, 204, 113);
            HoverColor = Color.FromArgb(52, 52, 55);
        }

        /// <summary>
        /// Sets custom border color theme
        /// </summary>
        /// <param name="borderColor">Border color</param>
        /// <param name="focusColor">Focus color (optional)</param>
        public void SetBorderTheme(Color borderColor, Color? focusColor = null)
        {
            BorderColor = borderColor;
            if (focusColor.HasValue)
                FocusColor = focusColor.Value;
        }
    }
}
