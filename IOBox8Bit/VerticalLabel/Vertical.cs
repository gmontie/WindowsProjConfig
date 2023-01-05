using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VerticalLabel
{
    public partial class Vertical : System.Windows.Forms.Control
    {
        private String AutoText;

        //----------------------------------------------------------------------
        //
        //
        //[ToolboxBitmap(typeof(Vertical), "Vertical.ico")]
        public Vertical()
            : base()
        {
            InitializeComponent();
            RenderingMode = System.Drawing.Text.TextRenderingHint.SystemDefault;
            SetStyle(System.Windows.Forms.ControlStyles.Opaque, true);
            Transparent = false;
            BorderColor = SystemColors.ActiveBorder;
            BorderVisable = false;
            BorderWidth = 3;
            Transparent = false;
            CenterX = 0;
            CenterY = 0;
            base.AutoSize = false;
            base.ForeColor = SystemColors.Control;
        }

        //----------------------------------------------------------------------
        //
        //
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Pen BorderPen;
            SolidBrush BackGroundColorBrush;
            SolidBrush ForeGoundColorBrush = new SolidBrush(ForeColor);

            if (BorderVisable)
                BorderPen = new Pen(BorderColor, BorderWidth);
            else
                BorderPen = new Pen(BackColor, 0);

            if (Transparent)
            {
                BackGroundColorBrush = new SolidBrush(Color.Empty);
            }
            else
            {
                BackGroundColorBrush = new SolidBrush(BackColor);
            }

            base.OnPaint(e);

            e.Graphics.DrawRectangle(BorderPen, 0, 0, Size.Width, Size.Height);
            e.Graphics.FillRectangle(BackGroundColorBrush, 0, 0, Size.Width, Size.Height);
            e.Graphics.TextRenderingHint = RenderingMode;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            if (base.RightToLeft == System.Windows.Forms.RightToLeft.No)
            {
                TransformY = Size.Height;
                e.Graphics.TranslateTransform(CenterX, TransformY);
                e.Graphics.RotateTransform(270);
                e.Graphics.DrawString(Text, Font, ForeGoundColorBrush, CenterX, CenterY);
            }
            else
            {
                TransformX = Size.Width - 5;
                e.Graphics.TranslateTransform(TransformX, CenterY);
                e.Graphics.RotateTransform(90);
                e.Graphics.DrawString(Text, Font, ForeGoundColorBrush, CenterX, CenterY, StringFormat.GenericTypographic);
            }
        }

        //----------------------------------------------------------------------
        //
        //
        private void VerticalLabel_Resize(object sender, System.EventArgs e)
        {
            Invalidate();
        }

        //----------------------------------------------------------------------
        //
        //
        public override String Text
        {
            get { return AutoText; }
            set
            {
                AutoText = value;
                Invalidate();
            }
        }

        public int CenterX { get; set; }
        public int CenterY { get; set; }
        private float TransformX { get; set; }
        private float TransformY { get; set; }
        public Boolean Transparent { get; set; }
        public System.Drawing.Text.TextRenderingHint RenderingMode { get; set; }
        public System.Drawing.StringFormat DrawFormat { get; set; }
        public Color BorderColor { get; set; }
        public Boolean BorderVisable { get; set; }
        public int BorderWidth { get; set; }
    }
}
