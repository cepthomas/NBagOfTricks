using System;
using System.Drawing;
using System.Windows.Forms;


namespace NBagOfTricks.UI
{
    /// <summary>
    /// Slider control.
    /// </summary>
    public partial class Slider : UserControl
    {
        #region Fields
        double _value = 0.0;
        double _resetVal = 0.0;
        #endregion

        #region Properties
        /// <summary>
        /// Optional label.
        /// </summary>
        public string Label { get; set; } = "";

        /// <summary>
        /// For styling.
        /// </summary>
        public Color ControlColor { get; set; } = Color.Orange;

        /// <summary>
        /// For styling.
        /// </summary>
        public override Color BackColor { get; set; } = SystemColors.Control;

        /// <summary>
        /// Maximum value of this slider.
        /// </summary>
        public double Maximum { get; set; } = 1.0;

        /// <summary>
        /// Minimum value of this slider.
        /// </summary>
        public double Minimum { get; set; } = 0.0;

        /// <summary>
        /// Reset value of this slider.
        /// </summary>
        public double ResetValue
        {
            get
            {
                return _resetVal;
            }
            set
            {
                _resetVal = MathUtils.Constrain(value, Minimum, Maximum);
            }
        }

        /// <summary>
        /// The value for this slider.
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = MathUtils.Constrain(value, Minimum, Maximum);
                ValueChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        /// <summary>
        /// Number of decimal places to display.
        /// </summary>
        public int DecPlaces { get; set; } = 1;
        #endregion

        #region Events
        /// <summary>
        /// Slider value changed event.
        /// </summary>
        public event EventHandler ValueChanged;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Creates a new Slider control.
        /// </summary>
        public Slider()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }
        #endregion

        #region Designer boilerplate
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Draw the slider.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Setup.
            pe.Graphics.Clear(BackColor);
            Brush brush = new SolidBrush(ControlColor);
            Pen pen = new Pen(ControlColor);

            // Draw border.
            int bw = 1;
            Pen penBorder = new Pen(Color.Black, bw);
            pe.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            // Draw data.
            Rectangle drawArea = Rectangle.Inflate(ClientRectangle, -bw, -bw);

            double x = Width * (_value - Minimum) / (Maximum - Minimum);
            pe.Graphics.FillRectangle(brush, bw, bw, (float)x - 2 * bw, Height - 2 * bw);

            // Text.
            StringFormat format = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
            string sval = _value.ToString("#0." + new string('0', DecPlaces));

            if (Label != "")
            {
                Rectangle r = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height / 2);
                pe.Graphics.DrawString(Label, Font, Brushes.Black, r, format);
                r = new Rectangle(ClientRectangle.X, ClientRectangle.Height / 2, ClientRectangle.Width, ClientRectangle.Height / 2);
                pe.Graphics.DrawString(sval, Font, Brushes.Black, r, format);
            }
            else
            {
                pe.Graphics.DrawString(sval, Font, Brushes.Black, ClientRectangle, format);
            }
        }
        #endregion

        #region Mouse events
        /// <summary>
        /// Handle dragging.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SetValueFromMouse(e.X);
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handle dragging.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            switch(e.Button)
            {
                case MouseButtons.Left:
                    SetValueFromMouse(e.X);
                    break;

                case MouseButtons.Right:
                    Value = ResetValue;
                    break;
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// ommon updater.
        /// </summary>
        /// <param name="x"></param>
        private void SetValueFromMouse(int x)
        {
            double newval = Minimum + x * (Maximum - Minimum) / Width;
            Value = MathUtils.Constrain(newval, Minimum, Maximum);
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Slider
            // 
            this.Name = "Slider";
            this.Size = new System.Drawing.Size(119, 44);
            this.ResumeLayout(false);

        }
    }
}
