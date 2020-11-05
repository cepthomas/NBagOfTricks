using System;
using System.Drawing;
using System.Windows.Forms;
using NBagOfTricks.Utils;


namespace NBagOfTricks.UI
{
    /// <summary>
    /// Pan slider control
    /// </summary>
    public partial class Pan : UserControl
    {
        #region Fields
        private double _value;
        #endregion

        #region Properties
        /// <summary>
        /// The current Pan setting.
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = Math.Round(MathUtils.Constrain(value, -1.0, 1.0), 2);
                ValueChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
        }

        /// <summary>
        /// For styling.
        /// </summary>
        public Color ControlColor { get; set; } = Color.Orange;
        #endregion

        #region Events
        /// <summary>
        /// True when pan value changed.
        /// </summary>
        public event EventHandler ValueChanged;
        #endregion

        /// <summary>
        /// Creates a new PanSlider control.
        /// </summary>
        public Pan()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Draw control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Setup.
            Brush brush = new SolidBrush(ControlColor);

            // Draw border.
            int bw = 1;
            Pen penBorder = new Pen(Color.Black, bw);
            pe.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            // Draw data.
            Rectangle drawArea = Rectangle.Inflate(ClientRectangle, -bw, -bw);
            string panValue;
            if (_value == 0.0)
            {
                pe.Graphics.FillRectangle(brush, (Width / 2) - bw, bw, 2 * bw, Height - 2 * bw);
                panValue = "C";
            }
            else if (_value > 0)
            {
                pe.Graphics.FillRectangle(brush, (Width / 2), bw, (int)((Width / 2) * _value), Height - 2 * bw);
                panValue = $"{_value * 100:F0}%R";
            }
            else
            {
                pe.Graphics.FillRectangle(brush, (int)((Width / 2) * (_value + bw)), bw, (int)((Width / 2) * (0 - _value)), Height - 2 * bw);
                panValue = $"{_value * -100:F0}%L";
            }

            // Draw text.
            StringFormat format = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
            pe.Graphics.DrawString(panValue, Font, Brushes.Black, ClientRectangle, format);
        }

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
            switch (e.Button)
            {
                case MouseButtons.Left:
                    SetValueFromMouse(e);
                    break;

                case MouseButtons.Right:
                    Value = 0;
                    break;
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Calculate position.
        /// </summary>
        /// <param name="e"></param>
        void SetValueFromMouse(MouseEventArgs e)
        {
            Value = ((double)e.X / Width * 2.0f) - 1.0f;
        }

        /// <summary>
        /// Handle the nudge key.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Down)
                {
                    Value = Value - 0.01f;
                }
                else if (e.KeyCode == Keys.Up)
                {
                    Value = Value + 0.01f;
                }
            }

            base.OnKeyDown(e);
        }


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
    }
}
