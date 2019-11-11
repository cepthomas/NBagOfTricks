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
        /// Number of decimal places to display.
        /// </summary>
        public int DecPlaces { get; set; } = 1;

        /// <summary>
        /// Fader orientation
        /// </summary>
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

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
                _resetVal = Math.Round(MathUtils.Constrain(value, Minimum, Maximum), DecPlaces);
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
                _value = Math.Round(MathUtils.Constrain(value, Minimum, Maximum), DecPlaces);
                ValueChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }
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
        System.ComponentModel.IContainer components = new System.ComponentModel.Container();

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
            int brdwidth = 1;

            // Draw data.
            Rectangle drawArea = Rectangle.Inflate(ClientRectangle, -brdwidth, -brdwidth);

            // Draw the bar.
            if (Orientation == Orientation.Horizontal)
            {
                double x = (_value - Minimum) / (Maximum - Minimum);
                pe.Graphics.FillRectangle(brush, drawArea.Left, drawArea.Top, drawArea.Width * (float)x, drawArea.Height);
            }
            else
            {
                double y = 1.0 - (_value - Minimum) / (Maximum - Minimum);
                pe.Graphics.FillRectangle(brush, drawArea.Left, drawArea.Height * (float)y, drawArea.Width, drawArea.Bottom);
            }

            // Draw border.
            Pen penBorder = new Pen(Color.Black, brdwidth);
            pe.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);
            pe.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            // Text.
            string sval = _value.ToString("#0." + new string('0', DecPlaces));
            StringFormat format = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };

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
                SetValueFromMouse(e);
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
                    SetValueFromMouse(e);
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
        /// <param name="e"></param>
        private void SetValueFromMouse(MouseEventArgs e)
        {
            double newval = Orientation == Orientation.Horizontal ?
                Minimum + e.X * (Maximum - Minimum) / Width :
                Minimum + (Height - e.Y) * (Maximum - Minimum) / Height;

            Value = newval;
        }

        /// <summary>
        /// Handle the nudge key.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control)
            {
                double incr = Math.Pow(10, -DecPlaces);

                if (e.KeyCode == Keys.Down)
                {
                    Value = Value - incr;
                }
                else if (e.KeyCode == Keys.Up)
                {
                    Value = Value + incr;
                }
            }

            base.OnKeyDown(e);
        }
        #endregion
    }
}
