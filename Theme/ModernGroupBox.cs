using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ServerManager
{
    [ToolboxItem(true)]
    public class ModernGroupBox : GroupBox
    {
        private Color _borderColor = Color.FromArgb(70, 70, 70);
        private Color _headerColor = Color.FromArgb(39, 174, 96);
        private int _cornerRadius = 8;

        [Category("Appearance")]
        [System.ComponentModel.DefaultValue(typeof(Color), "70, 70, 70")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [System.ComponentModel.DefaultValue(typeof(Color), "39, 174, 96")]
        public Color HeaderColor
        {
            get { return _headerColor; }
            set { _headerColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [System.ComponentModel.DefaultValue(8)]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set { _cornerRadius = value; Invalidate(); }
        }

        public ModernGroupBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.FromArgb(58, 58, 58);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Padding = new Padding(8, 30, 8, 8);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent?.BackColor ?? Color.FromArgb(45, 45, 48));

            // Mittaa otsikon koko tarkemmin
            var headerSize = e.Graphics.MeasureString(Text, Font);
            // Lisää riittävästi tilaa ettei teksti katkea
            var headerRect = new Rectangle(10, 0, (int)Math.Ceiling(headerSize.Width) + 24, (int)headerSize.Height + 4);

            // Piirrä ryhmän tausta
            var groupRect = new Rectangle(0, headerRect.Height / 2, Width, Height - headerRect.Height / 2);
            using (var brush = new SolidBrush(BackColor))
            {
                var path = CreateRoundedRectangle(groupRect, _cornerRadius);
                e.Graphics.FillPath(brush, path);
            }

            // Piirrä reunaviiva
            using (var pen = new Pen(_borderColor, 2))
            {
                var path = CreateRoundedRectangle(groupRect, _cornerRadius);
                e.Graphics.DrawPath(pen, path);
            }

            // Piirrä otsikon tausta
            using (var headerBrush = new SolidBrush(_headerColor))
            {
                var headerPath = CreateRoundedRectangle(headerRect, 4);
                e.Graphics.FillPath(headerBrush, headerPath);
            }

            // Piirrä otsikkoteksti
            using (var textBrush = new SolidBrush(ForeColor))
            {
                var textRect = new Rectangle(headerRect.X + 5, headerRect.Y + 2, headerRect.Width - 10, headerRect.Height - 4);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(Text, Font, textBrush, textRect, sf);
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            if (cornerRadius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.X, rect.Y, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius, rect.Y, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(rect.X + rect.Width - cornerRadius, rect.Y + rect.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();
            return path;
        }
    }
}