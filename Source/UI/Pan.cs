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
        /// <summary> </summary>
        private double _value;

        /// <summary>The pen.</summary>
        readonly Pen _pen = new Pen(Color.Black, 1);

        /// <summary>The brush.</summary>
        readonly SolidBrush _brush = new SolidBrush(Color.White);
        #endregion

        #region Properties
        /// <summary>The current Pan setting.</summary>
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

        /// <summary>For styling.</summary>
        public Color DrawColor { get { return _brush.Color; } set { _brush.Color = value; } }
        #endregion

        #region Events
        /// <summary>True when pan value changed.</summary>
        public event EventHandler ValueChanged;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Creates a new PanSlider control.
        /// </summary>
        public Pan()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Load += Pan_Load;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Pan_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pen.Dispose();
                _brush.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        /// <summary>
        /// Draw control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Setup.
            pe.Graphics.Clear(BackColor);

            // Draw border.
            pe.Graphics.DrawRectangle(_pen, 0, 0, Width - _pen.Width, Height - _pen.Width);

            // Draw data.
            string panValue;
            if (_value == 0.0)
            {
                pe.Graphics.FillRectangle(_brush, (Width / 2) - _pen.Width, _pen.Width, 2 * _pen.Width, Height - 2 * _pen.Width);
                panValue = "C";
            }
            else if (_value > 0)
            {
                pe.Graphics.FillRectangle(_brush, (Width / 2), _pen.Width, (int)((Width / 2) * _value), Height - 2 * _pen.Width);
                panValue = $"{_value * 100:F0}%R";
            }
            else
            {
                pe.Graphics.FillRectangle(_brush, (int)((Width / 2) * (_value + _pen.Width)), _pen.Width, (int)((Width / 2) * (0 - _value)), Height - 2 * _pen.Width);
                panValue = $"{_value * -100:F0}%L";
            }

            // Draw text.
            using (StringFormat format = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center })
            {
                pe.Graphics.DrawString(panValue, Font, Brushes.Black, ClientRectangle, format);
            }
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
                    Value -= 0.01f;
                }
                else if (e.KeyCode == Keys.Up)
                {
                    Value += 0.01f;
                }
            }

            base.OnKeyDown(e);
        }
    }
}
