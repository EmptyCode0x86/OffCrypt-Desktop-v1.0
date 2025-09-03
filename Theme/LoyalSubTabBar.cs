using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace OffCrypt
{
    /// <summary>
    /// LoyalSubTabBar – itsenäinen "mini-TabControl":
    /// - Oma Pages-kokoelma (kuten TabControl.TabPages)
    /// - Klikkaus vaihtaa SelectedIndex:in ja näyttää vastaavan sivun _contentHostissa
    /// - Designer-ystävällinen: Pages muokattavissa Property Gridistä, sivut designoitavissa
    /// - Otsikon muokkaus: voit muuttaa sivun Text-ominaisuutta Collection Editorissa ja Property Gridissä
    /// - Automaattinen napin leveys: painikkeen leveys mukautuu tekstin pituuteen (TextRenderer)
    /// - Dynaaminen korkeus: itemHeight skaalautuu vähintään fontin korkeuden mukaan
    /// </summary>
    [DefaultEvent("SelectedIndexChanged")]
    [ToolboxItem(true)]
    [Designer(typeof(LoyalSubTabBarDesigner))]
    [Description("Compact sub tab control with built-in pages (like a small TabControl)")]
    public class LoyalSubTabBar : Control
    {
        // ==== Visual defaults ====
        private Color _backColor = Color.FromArgb(31, 31, 31);
        private Color _itemBack = Color.FromArgb(50, 50, 50);
        private Color _itemHover = Color.FromArgb(64, 64, 64);
        private Color _itemSelected = Color.FromArgb(39, 174, 96);
        private Color _border = Color.FromArgb(70, 70, 70);
        private Color _text = Color.White;
        private Color _textDisabled = Color.FromArgb(170, 170, 170);

        // ==== Layout ====
        private int _itemHeight = 26;         // bar button height (min), skaalataan fontin mukaan
        private int _cornerRadius = 8;
        private int _itemSpacing = 6;
        private Padding _contentPadding = new Padding(6, 2, 6, 4);
        private int _itemPadX = 12;           // horizontal text padding in item
        private int _contentTopMargin = 6;    // spacing between bar and page content

        private readonly List<Rectangle> _itemBounds = new List<Rectangle>();
        private int _selectedIndex = -1;
        private int _hoverIndex = -1;

        // Content host panel (where pages live)
        private readonly Panel _contentHost;
        private SubTabPageCollection _pages;

        // Designerille näkyviin
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal Panel ContentHost => _contentHost;

        public LoyalSubTabBar()
        {
            System.Diagnostics.Debug.WriteLine("LoyalSubTabBar constructor called");
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = _backColor;
            Font = new Font("Segoe UI", 9f, FontStyle.Regular);

            _contentHost = new Panel
            {
                BackColor = Color.FromArgb(28, 28, 28),
                Dock = DockStyle.None
            };
            Controls.Add(_contentHost);

            Height = 220; // tilaa sisällölle
            TabStop = true;
        }

        // ===== Apurit =====
        [Browsable(false)]
        public int EffectiveItemHeight => Math.Max(_itemHeight, Font.Height + 8);

        [Browsable(false)]
        public int PreferredBarHeight => _contentPadding.Vertical + EffectiveItemHeight;

        // ===== Public API =====
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [MergableProperty(false)]
        [Editor(typeof(SubTabPageCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public SubTabPageCollection Pages
        {
            get { return _pages ?? (_pages = new SubTabPageCollection(this)); }
        }

        [Category("Behavior")]
        [Description("Zero-based index of the selected page. -1 when none selected.")]
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                int clamped = (Pages.Count == 0) ? -1 : Math.Max(-1, Math.Min(Pages.Count - 1, value));
                if (_selectedIndex == clamped) return;
                _selectedIndex = clamped;
                UpdatePageVisibility();
                Invalidate();
                OnSelectedIndexChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SubTabPage SelectedPage => (_selectedIndex >= 0 && _selectedIndex < Pages.Count) ? Pages[_selectedIndex] : null;

        // ⭐ Helppo otsikon muokkaus Property Gridistä (proxy valitun sivun Textiin)
        [Category("Appearance")]
        [Description("Title of the currently selected page (proxy to SelectedPage.Text)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedPageText
        {
            get => SelectedPage?.Text ?? string.Empty;
            set { if (SelectedPage != null) { SelectedPage.Text = value; Invalidate(new Rectangle(0, 0, Width, PreferredBarHeight)); } }
        }

        // ==== Appearance properties with designer metadata ====
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "50, 50, 50")]
        public Color ItemBackColor { get => _itemBack; set { _itemBack = value; Invalidate(); } }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "64, 64, 64")]
        public Color ItemHoverColor { get => _itemHover; set { _itemHover = value; Invalidate(); } }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "39, 174, 96")]
        public Color ItemSelectedColor { get => _itemSelected; set { _itemSelected = value; Invalidate(); } }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "70, 70, 70")]
        public Color ItemBorderColor { get => _border; set { _border = value; Invalidate(); } }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "255, 255, 255")]
        public Color ItemTextColor { get => _text; set { _text = value; Invalidate(); } }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "170, 170, 170")]
        public Color ItemTextDisabledColor { get => _textDisabled; set { _textDisabled = value; Invalidate(); } }

        [Category("Layout")]
        [DefaultValue(26)]
        public int ItemHeight { get => _itemHeight; set { _itemHeight = Math.Max(18, Math.Min(48, value)); PerformLayout(); Invalidate(); } }

        [Category("Layout")]
        [DefaultValue(8)]
        public int CornerRadius { get => _cornerRadius; set { _cornerRadius = Math.Max(0, Math.Min(24, value)); Invalidate(); } }

        [Category("Layout")]
        [DefaultValue(6)]
        public int ItemSpacing { get => _itemSpacing; set { _itemSpacing = Math.Max(0, Math.Min(24, value)); PerformLayout(); Invalidate(); } }

        [Category("Layout")]
        [DefaultValue(typeof(Padding), "6, 2, 6, 4")]
        public Padding ContentPadding { get => _contentPadding; set { _contentPadding = value; PerformLayout(); Invalidate(); } }

        [Category("Layout")]
        [DefaultValue(12)]
        public int ItemPaddingX { get => _itemPadX; set { _itemPadX = Math.Max(4, Math.Min(48, value)); PerformLayout(); Invalidate(); } }

        [Category("Layout")]
        [DefaultValue(6)]
        public int ContentTopMargin { get => _contentTopMargin; set { _contentTopMargin = Math.Max(0, Math.Min(32, value)); PerformLayout(); Invalidate(); } }

        // ===== Events =====
        public event EventHandler SelectedIndexChanged;
        public event EventHandler PagesChanged; // designerin kuunteluun
        protected virtual void OnSelectedIndexChanged(EventArgs e) => SelectedIndexChanged?.Invoke(this, e);
        protected virtual void OnPagesChanged() => PagesChanged?.Invoke(this, EventArgs.Empty);

        // ===== ShouldSerialize/Reset (designer analyzers) =====
        public bool ShouldSerializeItemBackColor() => _itemBack != Color.FromArgb(50, 50, 50);
        public void ResetItemBackColor() => ItemBackColor = Color.FromArgb(50, 50, 50);
        public bool ShouldSerializeItemHoverColor() => _itemHover != Color.FromArgb(64, 64, 64);
        public void ResetItemHoverColor() => ItemHoverColor = Color.FromArgb(64, 64, 64);
        public bool ShouldSerializeItemSelectedColor() => _itemSelected != Color.FromArgb(39, 174, 96);
        public void ResetItemSelectedColor() => ItemSelectedColor = Color.FromArgb(39, 174, 96);
        public bool ShouldSerializeItemBorderColor() => _border != Color.FromArgb(70, 70, 70);
        public void ResetItemBorderColor() => ItemBorderColor = Color.FromArgb(70, 70, 70);
        public bool ShouldSerializeItemTextColor() => _text != Color.FromArgb(255, 255, 255);
        public void ResetItemTextColor() => ItemTextColor = Color.FromArgb(255, 255, 255);
        public bool ShouldSerializeItemTextDisabledColor() => _textDisabled != Color.FromArgb(170, 170, 170);
        public void ResetItemTextDisabledColor() => ItemTextDisabledColor = Color.FromArgb(170, 170, 170);
        public bool ShouldSerializeContentPadding() => _contentPadding != new Padding(6, 2, 6, 4);
        public void ResetContentPadding() => ContentPadding = new Padding(6, 2, 6, 4);

        public bool ShouldSerializePages() => Pages.Count > 0;
        public void ResetPages() => Pages.Clear();

        // ===== Internals =====
        internal void NotifyPagesChanged()
        {
            EnsurePageParenting();
            if (_selectedIndex >= Pages.Count) _selectedIndex = Pages.Count - 1;
            if (_selectedIndex < -1) _selectedIndex = -1;
            if (_selectedIndex == -1 && Pages.Count > 0) _selectedIndex = 0;
            UpdatePageVisibility();
            
            // Varmista että kaikki SubTabPage:t ovat muiden kontrollien edessä
            if (_contentHost != null)
            {
                foreach (var page in Pages.Cast<SubTabPage>())
                {
                    if (page.Visible)
                    {
                        page.BringToFront();
                    }
                }
            }
            
            PerformLayout();
            Invalidate(new Rectangle(0, 0, Width, PreferredBarHeight));
            OnPagesChanged();
        }

        private void EnsurePageParenting()
        {
            foreach (var p in Pages.Cast<SubTabPage>())
            {
                if (p.Parent != _contentHost)
                {
                    p.Parent = _contentHost;
                    p.Dock = DockStyle.Fill;
                    p.Visible = false;
                }
            }
        }

        private void UpdatePageVisibility()
        {
            if (_contentHost == null) return;
            
            // Debug-lokitus
            System.Diagnostics.Debug.WriteLine($"UpdatePageVisibility: SelectedIndex={_selectedIndex}, Pages.Count={Pages.Count}");
            
            // Ensin piilota kaikki sivut ja lähetä taakse
            for (int i = 0; i < Pages.Count; i++)
            {
                var page = Pages[i];
                if (i != _selectedIndex)
                {
                    page.Visible = false;
                    page.SendToBack();
                    System.Diagnostics.Debug.WriteLine($"  Page {i} ({page.Text}): Hidden and sent to back");
                }
            }
            
            // Sitten näytä ja tuo eteen valittu sivu
            if (_selectedIndex >= 0 && _selectedIndex < Pages.Count)
            {
                var selectedPage = Pages[_selectedIndex];
                selectedPage.Visible = true;
                selectedPage.BringToFront();
                System.Diagnostics.Debug.WriteLine($"  Page {_selectedIndex} ({selectedPage.Text}): Visible and brought to front");
                
                // Varmista että kaikki lapsikonttrollit ovat näkyvissä
                foreach (Control child in selectedPage.Controls)
                {
                    if (!child.Visible)
                    {
                        child.Visible = true;
                        System.Diagnostics.Debug.WriteLine($"    Child control {child.Name}: Made visible");
                    }
                    child.BringToFront();
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutContentHost();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            PerformLayout();
            Invalidate(new Rectangle(0, 0, Width, PreferredBarHeight));
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            LayoutItems();
            LayoutContentHost();
        }

        private void LayoutContentHost()
        {
            int barHeight = PreferredBarHeight;
            _contentHost.Bounds = new Rectangle(0, barHeight + _contentTopMargin, Width, Math.Max(0, Height - (barHeight + _contentTopMargin)));
        }

        private void LayoutItems()
        {
            _itemBounds.Clear();
            if (Pages.Count == 0) return;

            int x = _contentPadding.Left;
            int y = _contentPadding.Top;

            foreach (var pg in Pages.Cast<SubTabPage>())
            {
                string text = pg.Text ?? string.Empty;
                Size sz = TextRenderer.MeasureText(text, Font, new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPadding);
                int w = Math.Max(24, sz.Width + (_itemPadX * 2));
                var rect = new Rectangle(x, y, w, EffectiveItemHeight);
                _itemBounds.Add(rect);
                x += w + _itemSpacing;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var bg = new SolidBrush(_backColor)) g.FillRectangle(bg, new Rectangle(0, 0, Width, PreferredBarHeight));

            if (Pages.Count == 0)
            {
                if (IsDesignHost())
                {
                    using (var hint = new SolidBrush(Color.FromArgb(140, Color.White)))
                    using (var f = new Font(Font, FontStyle.Italic))
                    {
                        TextRenderer.DrawText(g, "Add Pages in the property grid…", f, new Rectangle(0, 0, Width, PreferredBarHeight),
                            Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    }
                }
                return;
            }

            for (int i = 0; i < _itemBounds.Count; i++)
            {
                var rect = _itemBounds[i];
                var page = Pages[i];
                bool selected = (i == _selectedIndex);
                bool hovered = (i == _hoverIndex);

                Color fill = selected ? _itemSelected : hovered ? _itemHover : _itemBack;
                Color txt = _text;

                using (var path = Rounded(rect, _cornerRadius))
                using (var fillB = new SolidBrush(fill))
                using (var pen = new Pen(_border))
                {
                    g.FillPath(fillB, path);
                    g.DrawPath(pen, path);
                }

                TextRenderer.DrawText(g, page.Text ?? string.Empty, Font, rect, txt,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            // Border line under bar
            using (var pen = new Pen(_border))
            {
                g.DrawLine(pen, 0, PreferredBarHeight, Width, PreferredBarHeight);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int idx = HitTest(e.Location);
            if (idx != _hoverIndex)
            {
                _hoverIndex = idx;
                Invalidate(new Rectangle(0, 0, Width, PreferredBarHeight));
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverIndex != -1) { _hoverIndex = -1; Invalidate(new Rectangle(0, 0, Width, PreferredBarHeight)); }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            // AINA debug jokaisesta klikkauksesta
            System.Diagnostics.Debug.WriteLine($"OnMouseDown: Button={e.Button}, Location=({e.X},{e.Y}), PreferredBarHeight={PreferredBarHeight}");
            
            if (e.Button != MouseButtons.Left) 
            {
                System.Diagnostics.Debug.WriteLine("  Not left button, ignoring");
                return;
            }
            
            if (e.Y > PreferredBarHeight) 
            {
                System.Diagnostics.Debug.WriteLine($"  Y ({e.Y}) > PreferredBarHeight ({PreferredBarHeight}), ignoring");
                return; // clicks only on bar
            }
            
            int idx = HitTest(e.Location);
            System.Diagnostics.Debug.WriteLine($"  HitTest returned {idx}, current SelectedIndex={_selectedIndex}, Pages.Count={Pages.Count}");
            
            if (idx >= 0 && idx < Pages.Count)
            {
                System.Diagnostics.Debug.WriteLine($"  Changing SelectedIndex from {_selectedIndex} to {idx}");
                SelectedIndex = idx;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"  idx {idx} out of range [0, {Pages.Count})");
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (Pages.Count == 0) return;
            if (e.KeyCode == Keys.Left)
            {
                int i = SelectedIndex <= 0 ? Pages.Count - 1 : SelectedIndex - 1;
                SelectedIndex = i;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Right)
            {
                int i = SelectedIndex >= Pages.Count - 1 ? 0 : SelectedIndex + 1;
                SelectedIndex = i;
                e.Handled = true;
            }
        }

        private int HitTest(Point p)
        {
            for (int i = 0; i < _itemBounds.Count; i++)
            {
                if (_itemBounds[i].Contains(p)) return i;
            }
            return -1;
        }

        private static GraphicsPath Rounded(Rectangle r, int radius)
        {
            int d = Math.Max(0, radius * 2);
            var gp = new GraphicsPath();
            if (d == 0) { gp.AddRectangle(r); gp.CloseFigure(); return gp; }
            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }

        private bool IsDesignHost()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;
            if (Site?.DesignMode == true) return true;
            Control p = this;
            while (p != null)
            {
                if (p.Site?.DesignMode == true) return true;
                p = p.Parent;
            }
            return false;
        }
    }

    // ===== Page model =====
    [Designer(typeof(ParentControlDesigner))]
    [ToolboxItem(false)]
    public class SubTabPage : Panel
    {
        public SubTabPage()
        {
            BackColor = Color.FromArgb(28, 28, 28);
            ForeColor = Color.White;
            Dock = DockStyle.Fill;
        }

        // Näytetään Text-ominaisuus kokoelmaeditorissa ja property gridissä
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        [Bindable(true)]
        [DefaultValue("")]
        public override string Text
        {
            get => base.Text;
            set { base.Text = value; Invalidate(); }
        }

        public override string ToString() => string.IsNullOrWhiteSpace(Text) ? base.ToString() : Text;
    }

    // ===== Collection =====
    public class SubTabPageCollection : CollectionBase
    {
        private readonly LoyalSubTabBar _owner;
        internal SubTabPageCollection(LoyalSubTabBar owner) { _owner = owner; }

        public int Count => List.Count;

        public SubTabPage this[int index]
        {
            get => (SubTabPage)List[index];
            set
            {
                var old = (SubTabPage)List[index];
                Detach(old);
                List[index] = value;
                Attach(value);
                _owner?.NotifyPagesChanged();
            }
        }

        public int Add(SubTabPage page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));
            int i = List.Add(page);
            Attach(page);
            _owner?.NotifyPagesChanged();
            return i;
        }

        public void Add(string text)
        {
            Add(new SubTabPage { Text = text });
        }

        public void AddRange(IEnumerable<SubTabPage> pages)
        {
            foreach (var it in pages) { List.Add(it); Attach(it); }
            _owner?.NotifyPagesChanged();
        }

        public void Insert(int index, SubTabPage page)
        {
            List.Insert(index, page);
            Attach(page);
            _owner?.NotifyPagesChanged();
        }

        public void Remove(SubTabPage page)
        {
            if (page == null) return;
            if (page.Parent is Control parent) parent.Controls.Remove(page);
            Detach(page);
            List.Remove(page);
            _owner?.NotifyPagesChanged();
        }

        protected override void OnClear()
        {
            base.OnClear();
            _owner?.NotifyPagesChanged();
        }

        private void Attach(SubTabPage p)
        {
            if (p == null) return;
            p.TextChanged += Page_TextChanged;
        }

        private void Detach(SubTabPage p)
        {
            if (p == null) return;
            p.TextChanged -= Page_TextChanged;
        }

        private void Page_TextChanged(object sender, EventArgs e)
        {
            _owner?.NotifyPagesChanged(); // relayout + repaint kun otsikko vaihtuu
        }

        public IEnumerator<SubTabPage> GetEnumerator()
        {
            foreach (var o in List) yield return (SubTabPage)o;
        }
    }

    // ===== Designer: mahdollistaa sivujen valinnan ja designoinnin =====
    public class LoyalSubTabBarDesigner : ParentControlDesigner
    {
        private LoyalSubTabBar _bar;
        private ISelectionService _selectionService;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _bar = component as LoyalSubTabBar;
            _selectionService = (ISelectionService)GetService(typeof(ISelectionService));
            Hook();
            EnableDesignModes();
        }

        private void Hook()
        {
            if (_bar == null) return;
            _bar.SelectedIndexChanged += Bar_SelectedIndexChanged;
            _bar.PagesChanged += Bar_PagesChanged;
        }

        private void Bar_PagesChanged(object sender, EventArgs e)
        {
            EnableDesignModes();
        }

        private void Bar_SelectedIndexChanged(object sender, EventArgs e)
        {
            var page = _bar.SelectedPage;
            if (page != null)
            {
                _selectionService?.SetSelectedComponents(new IComponent[] { page }, SelectionTypes.Primary);
            }
        }

        private void EnableDesignModes()
        {
            if (_bar == null) return;
            try { EnableDesignMode(_bar.ContentHost, "ContentHost"); } catch { /* ignore */ }
            foreach (var p in _bar.Pages.Cast<SubTabPage>())
            {
                try { EnableDesignMode(p, string.IsNullOrEmpty(p.Name) ? (p.Text ?? "Page") : p.Name); } catch { /* ignore */ }
            }
        }

        // Salli klikkausten mennä ohjaimelle, jotta se vaihtaa SelectedIndexin myös designerissä
        protected override bool GetHitTest(Point point)
        {
            if (_bar == null) return false;
            var client = _bar.PointToClient(point); // screen -> client
            return client.Y <= _bar.PreferredBarHeight;
        }
    }

    // ===== Collection editor =====
    public class SubTabPageCollectionEditor : CollectionEditor
    {
        public SubTabPageCollectionEditor(Type type) : base(type) { }
        protected override Type CreateCollectionItemType() => typeof(SubTabPage);
        protected override object CreateInstance(Type itemType)
        {
            return new SubTabPage { Text = "Page " + (DateTime.Now.Ticks % 1000) };
        }
    }
}
