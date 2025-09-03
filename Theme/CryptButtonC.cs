using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OffCrypt
{
    [ToolboxItem(true)]
    public class CryptButtonC : Button
    {
        // Väriominaisuudet
        private Color _borderColor = Color.FromArgb(70, 70, 70);
        private Color _primaryColor = Color.FromArgb(39, 174, 96); // Vihreä oletusväri
        private Color _hoverColor = Color.FromArgb(46, 204, 113); // Vaaleampi vihreä hover-tila
        private Color _pressedColor = Color.FromArgb(34, 153, 84); // Tummempi vihreä painettaessa
        private Color _textColor = Color.White;
        
        // Ulkoasun ominaisuudet
        private int _cornerRadius = 8;
        private bool _isHovering = false;
        private bool _isPressed = false;
        private bool _useBorder = true;
        private int _borderWidth = 1;
        private bool _useGradient = false;
        private Color _gradientColor = Color.Transparent;

        #region Ominaisuudet

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "70, 70, 70")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "39, 174, 96")]
        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set { _primaryColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "46, 204, 113")]
        public Color HoverColor
        {
            get { return _hoverColor; }
            set { _hoverColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "34, 153, 84")]
        public Color PressedColor
        {
            get { return _pressedColor; }
            set { _pressedColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color TextColorCustom
        {
            get { return _textColor; }
            set { _textColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(8)]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set { _cornerRadius = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool UseBorder
        {
            get { return _useBorder; }
            set { _useBorder = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool UseGradient
        {
            get { return _useGradient; }
            set { _useGradient = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color GradientColor
        {
            get { return _gradientColor; }
            set { _gradientColor = value; Invalidate(); }
        }

        #endregion

        public CryptButtonC()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            Size = new Size(140, 40);
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        #region Piirtologiikka

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Valitse taustaväri tilan mukaan
            Color buttonColor = _primaryColor;
            if (_isPressed)
                buttonColor = _pressedColor;
            else if (_isHovering)
                buttonColor = _hoverColor;

            // Piirrä painikkeen tausta
            Rectangle buttonRect = new Rectangle(0, 0, Width, Height);
            GraphicsPath buttonPath = CreateRoundedRectangle(buttonRect, _cornerRadius);

            // Jos käytetään liukuväriä
            if (_useGradient)
            {
                Color endColor = _gradientColor != Color.Transparent ? _gradientColor : GetDarkerColor(buttonColor, 0.15f);
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    buttonRect, buttonColor, endColor, LinearGradientMode.ForwardDiagonal))
                {
                    e.Graphics.FillPath(brush, buttonPath);
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(buttonColor))
                {
                    e.Graphics.FillPath(brush, buttonPath);
                }
            }

            // Piirrä reunaviiva jos käytössä
            if (_useBorder)
            {
                using (Pen pen = new Pen(_borderColor, _borderWidth))
                {
                    e.Graphics.DrawPath(pen, buttonPath);
                }
            }

            // Lisää valoefekti
            if (!_isPressed)
            {
                Rectangle highlightRect = new Rectangle(1, 1, Width - 2, Height / 2 - 2);
                GraphicsPath highlightPath = CreateRoundedRectangleTop(highlightRect, _cornerRadius);
                using (LinearGradientBrush highlightBrush = new LinearGradientBrush(
                    highlightRect,
                    Color.FromArgb(90, 255, 255, 255),
                    Color.FromArgb(5, 255, 255, 255),
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillPath(highlightBrush, highlightPath);
                }
            }

            // Tekstin sijainti ja piirto
            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // Piirrä teksti
            using (SolidBrush textBrush = new SolidBrush(_textColor))
            {
                e.Graphics.DrawString(Text, Font, textBrush, buttonRect, sf);
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);

            // Vasen yläkulma
            path.AddArc(arc, 180, 90);

            // Oikea yläkulma
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Oikea alakulma
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Vasen alakulma
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private GraphicsPath CreateRoundedRectangleTop(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rect.Location, size);

            // Vasen yläkulma
            path.AddArc(arc, 180, 90);

            // Oikea yläkulma
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Suorat sivut
            path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom);
            path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
            path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + radius);

            path.CloseFigure();
            return path;
        }

        #endregion

        #region Apufunktiot

        private Color GetDarkerColor(Color color, float percentage)
        {
            float red = color.R * (1 - percentage);
            float green = color.G * (1 - percentage);
            float blue = color.B * (1 - percentage);
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        private Color GetLighterColor(Color color, float percentage)
        {
            float red = color.R + ((255 - color.R) * percentage);
            float green = color.G + ((255 - color.G) * percentage);
            float blue = color.B + ((255 - color.B) * percentage);
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        #endregion

        #region Tapahtumat

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovering = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovering = false;
            _isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        #endregion

        #region Esimääritetyt Teemavärit

        /// <summary>
        /// Asettaa painikkeen vihreäksi teemaväriksi
        /// </summary>
        public void SetGreenTheme()
        {
            _primaryColor = Color.FromArgb(39, 174, 96);
            _hoverColor = Color.FromArgb(46, 204, 113);
            _pressedColor = Color.FromArgb(34, 153, 84);
            Invalidate();
        }

        /// <summary>
        /// Asettaa painikkeen siniseksi teemaväriksi
        /// </summary>
        public void SetBlueTheme()
        {
            _primaryColor = Color.FromArgb(52, 73, 94);
            _hoverColor = Color.FromArgb(52, 152, 219);
            _pressedColor = Color.FromArgb(44, 62, 80);
            Invalidate();
        }

        /// <summary>
        /// Asettaa painikkeen harmaaksi teemaväriksi
        /// </summary>
        public void SetGrayTheme()
        {
            _primaryColor = Color.FromArgb(127, 140, 141);
            _hoverColor = Color.FromArgb(149, 165, 166);
            _pressedColor = Color.FromArgb(93, 109, 126);
            Invalidate();
        }

        /// <summary>
        /// Asettaa painikkeen violetiksi teemaväriksi
        /// </summary>
        public void SetPurpleTheme()
        {
            _primaryColor = Color.FromArgb(142, 68, 173);
            _hoverColor = Color.FromArgb(155, 89, 182);
            _pressedColor = Color.FromArgb(108, 52, 131);
            Invalidate();
        }

        /// <summary>
        /// Asettaa painikkeen oranssiksi teemaväriksi
        /// </summary>
        public void SetOrangeTheme()
        {
            _primaryColor = Color.FromArgb(211, 84, 0);
            _hoverColor = Color.FromArgb(230, 126, 34);
            _pressedColor = Color.FromArgb(186, 74, 0);
            Invalidate();
        }

        /// <summary>
        /// Asettaa painikkeen tumman teemaväriksi
        /// </summary>
        public void SetDarkTheme()
        {
            _primaryColor = Color.FromArgb(45, 45, 48);
            _hoverColor = Color.FromArgb(65, 65, 68);
            _pressedColor = Color.FromArgb(40, 40, 43);
            Invalidate();
        }

        #endregion
    }
}