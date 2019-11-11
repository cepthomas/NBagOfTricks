using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NBagOfTricks.UI
{
    /// <summary></summary>
    public enum Taper { Linear, Log } // TODO Log sort of works but needs more debug. Also for slider?

    /// <summary>
    /// Control potentiometer.
    /// </summary>
    public partial class Pot : UserControl
    {
        #region Fields
        double _minimum = 0.0;
        double _maximum = 1.0;
        double _value = 0.5;
        double _beginDragValue = 0.0;
        int _beginDragY = 0;
        bool _dragging = false;
        #endregion

        #region Properties
        /// <summary>
        /// For styling.
        /// </summary>
        public Color ControlColor { get; set; } = Color.Black;

        /// <summary>
        /// Name etc.
        /// </summary>
        public string Label { get; set; } = "???";

        /// <summary>
        /// Taper.
        /// </summary>
        public Taper Taper { get; set; } = Taper.Linear;

        /// <summary>
        /// Number of decimal places to display.
        /// </summary>
        public int DecPlaces { get; set; } = 1;

        /// <summary>
        /// Minimum Value of the Pot.
        /// </summary>
        public double Minimum
        {
            get { return _minimum; }
            set { _minimum = Math.Min(value, _maximum); Invalidate(); }
        }

        /// <summary>
        /// Maximum Value of the Pot.
        /// </summary>
        public double Maximum
        {
            get { return _maximum; }
            set { _maximum = Math.Max(value, _minimum); Invalidate(); }
        }

        /// <summary>
        /// The current value of the pot.
        /// </summary>
        public double Value
        {
            get { return _value; }
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
        /// Value changed event.
        /// </summary>
        public event EventHandler ValueChanged;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Creates a new pot control.
        /// </summary>
        public Pot()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "Pot";
            Size = new Size(115, 86);
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
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            int diameter = Math.Min(Width - 4, Height - 4);

            Pen pen = new Pen(ControlColor, 3.0f)
            {
                LineJoin = System.Drawing.Drawing2D.LineJoin.Round
            };

            System.Drawing.Drawing2D.GraphicsState state = e.Graphics.Save();

            e.Graphics.Clear(BackColor);
            e.Graphics.TranslateTransform(Width / 2, Height / 2);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawArc(pen, new Rectangle(diameter / -2, diameter / -2, diameter, diameter), 135, 270);

            double val = Taper == Taper.Log ? Math.Log10(_value) : _value;
            double min = Taper == Taper.Log ? Math.Log10(_minimum) : _minimum;
            double max = Taper == Taper.Log ? Math.Log10(_maximum) : _maximum;
            double percent = (val - min) / (max - min);

            double degrees = 135 + (percent * 270);
            double x = (diameter / 2.0) * Math.Cos(Math.PI * degrees / 180);
            double y = (diameter / 2.0) * Math.Sin(Math.PI * degrees / 180);
            e.Graphics.DrawLine(pen, 0, 0, (float)x, (float)y);

            StringFormat format = new StringFormat()
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            Rectangle srect = new Rectangle(0, 7, 0, 0);
            string sValue = _value.ToString("#." + new string('0', DecPlaces));
            e.Graphics.DrawString(sValue, Font, Brushes.Black, srect, format);

            srect = new Rectangle(0, 20, 0, 0);
            e.Graphics.DrawString(Label, Font, Brushes.Black, srect, format);

            e.Graphics.Restore(state);
            base.OnPaint(e);
        }

        /// <summary>
        /// Handles the mouse down event to allow changing value by dragging.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _dragging = true;
            _beginDragY = e.Y;
            _beginDragValue = _value;
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Handles the mouse up event to allow changing value by dragging.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _dragging = false;
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Handles the mouse down event to allow changing value by dragging.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_dragging)
            {
                int ydiff = _beginDragY - e.Y; // pixels

                double val = Taper == Taper.Log ? Math.Log10(_value) : _value;
                double min = Taper == Taper.Log ? Math.Log10(_minimum) : _minimum;
                double max = Taper == Taper.Log ? Math.Log10(_maximum) : _maximum;
                double delta = (max - min) * (ydiff / 100.0);
                double newValue = MathUtils.Constrain(_beginDragValue + delta, min, max);
                Value = Taper == Taper.Log ? Math.Pow(newValue, 10) : newValue;
            }
            base.OnMouseMove(e);
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
