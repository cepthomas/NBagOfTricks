using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NBagOfTricks.Utils;


namespace NBagOfTricks.UI
{
    public partial class TimeControl : UserControl
    {
        #region Fields
        TimeSpan _length = new TimeSpan();
        TimeSpan _current = new TimeSpan();

        int LARGE_CHANGE = 1000;
        int SMALL_CHANGE = 100;
        int BORDER_WIDTH = 1;

        int _lastXPos = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Where we be now.
        /// </summary>
        public TimeSpan CurrentTime { get { return _current; } set { _current = value; Invalidate(); } }

        /// <summary>
        /// Where we be going.
        /// </summary>
        public TimeSpan Length { get { return _length; } set { _length = value; Invalidate(); } }

        /// <summary>
        /// For styling.
        /// </summary>
        public Color ControlColor { get; set; } = Color.Orange;

        /// <summary>
        /// 
        /// </summary>
        Font FontLarge { get; set; } = new Font("Consolas", 24, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>
        /// 
        /// </summary>
        Font FontSmall { get; set; } = new Font("Consolas", 14, FontStyle.Regular, GraphicsUnit.Point, 0);

        #endregion

        #region Events
        /// <summary>
        /// Value changed by user.
        /// </summary>
        public event EventHandler ValueChanged;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public TimeControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        /// <summary>
        /// Ready to show.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeControl_Load(object sender, EventArgs e)
        {
            toolTip.SetToolTip(this, "Current time");
            Invalidate();
        }
        #endregion

        /// <summary>
        /// Convert total msec into a TimeSpan.
        /// </summary>
        /// <param name="msec"></param>
        void AdjustTime(int msec)
        {
            if(msec > 0)
            {
                _current.Add(new TimeSpan(0, 0, 0, 0, msec));
            }
            else if(msec < 0)
            {
                _current.Subtract(new TimeSpan(0, 0, 0, 0, msec));
            }

            // Sanity checks.
            if (_current > _length)
            {
                _current = _length;
            }

            if (_current.TotalMilliseconds < 0)
            {
                _current = new TimeSpan();
            }
        }

        /// <summary>
        /// Pretty print a TimeSpan.
        /// </summary>
        /// <param name="ts"></param>
        string FormatTime(TimeSpan ts)
        {
            return $"{ts.TotalMinutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Add:
                case Keys.Up:
                    AdjustTime(e.Shift ? SMALL_CHANGE : LARGE_CHANGE);
                    e.IsInputKey = true;
                    break;

                case Keys.Subtract:
                case Keys.Down:
                    AdjustTime(e.Shift ? -SMALL_CHANGE : -LARGE_CHANGE);
                    e.IsInputKey = true;
                    break;
            }
        }

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
            Pen penBorder = new Pen(Color.Black, BORDER_WIDTH);
            pe.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            // Draw data.
            Rectangle drawArea = Rectangle.Inflate(ClientRectangle, -BORDER_WIDTH, -BORDER_WIDTH);

            if (_current < _length)
            {
                pe.Graphics.FillRectangle(brush,
                    BORDER_WIDTH,
                    BORDER_WIDTH,
                    (Width - 2 * BORDER_WIDTH) * (int)(_current.TotalMilliseconds / _length.TotalMilliseconds),
                    Height - 2 * BORDER_WIDTH);
            }

            // Text.
            StringFormat format = new StringFormat()
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };

            pe.Graphics.DrawString(FormatTime(_current), FontLarge, Brushes.Black, ClientRectangle, format);

            Rectangle r2 = new Rectangle(ClientRectangle.X + 66, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            pe.Graphics.DrawString(FormatTime(_length), FontSmall, Brushes.Black, r2, format);
        }
        #endregion

        #region UI handlers
        /// <summary>
        /// Handle mouse position changes.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SetCurrentTimeFromMouse(e.X);
            }
            else
            {
                if (e.X != _lastXPos)
                {
                    TimeSpan ts = GetTimeFromMouse(e.X);
                    toolTip.SetToolTip(this, FormatTime(ts));
                    _lastXPos = e.X;
                }
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handle dragging.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            SetCurrentTimeFromMouse(e.X);
            base.OnMouseDown(e);
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Common updater.
        /// </summary>
        /// <param name="x"></param>
        private void SetCurrentTimeFromMouse(int x)
        {
            _current = GetTimeFromMouse(x);
            ValueChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Convert x pos to msec.
        /// </summary>
        /// <param name="x"></param>
        private TimeSpan GetTimeFromMouse(int x)
        {
            double dval = MathUtils.Constrain(x * _current.TotalMilliseconds / _length.TotalMilliseconds / Width, 0, Width);
            return new TimeSpan(0, 0, 0, 0, (int)dval);
        }
        #endregion
    }
}
