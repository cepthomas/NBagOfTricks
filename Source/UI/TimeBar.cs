using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using NBagOfTricks.Utils;


namespace NBagOfTricks.UI
{
    public partial class TimeBar : UserControl // TODOC snap function?
    {
        #region Fields
        /// <summary>Total length.</summary>
        TimeSpan _lengthX = new TimeSpan();

        /// <summary>Current time/position.</summary>
        TimeSpan _current = new TimeSpan();

        /// <summary>One marker.</summary>
        TimeSpan _marker1 = new TimeSpan();

        /// <summary>Other marker.</summary>
        TimeSpan _marker2 = new TimeSpan();

        /// <summary>For tracking mouse moves.</summary>
        int _lastXPos = 0;

        /// <summary>Tooltip for mousing.</summary>
        readonly ToolTip toolTip = new ToolTip();

        /// <summary>The border pen.</summary>
        readonly Pen _penBorder = new Pen(Color.Black, 1);

        /// <summary>The marker pen.</summary>
        readonly Pen _penMarker = new Pen(Color.Black, 1);

        /// <summary>The brush.</summary>
        readonly SolidBrush _brush = new SolidBrush(Color.White);

        /// <summary>Constant.</summary>
        private static readonly int LARGE_CHANGE = 1000;

        /// <summary>Constant.</summary>
        private static readonly int SMALL_CHANGE = 100;
        #endregion

        #region Properties
        /// <summary>Where we be now.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public TimeSpan CurrentTime { get { return _current; } set { _current = value; UpdateTime(); } }

        /// <summary>Total length.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public TimeSpan Length { get { return _lengthX; } set { _lengthX = value; UpdateTime(); } }

        /// <summary>One marker.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public TimeSpan Marker1 { get { return _marker1; } set { _marker1 = value; UpdateTime(); } }

        /// <summary>Other marker.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public TimeSpan Marker2 { get { return _marker2; } set { _marker2 = value; UpdateTime(); } }

        /// <summary>For styling.</summary>
        public Color ProgressColor { get { return _brush.Color; } set { _brush.Color = value; } }

        /// <summary>Big font.</summary>
        Font FontLarge { get; set; } = new Font("Cascadia", 24, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>Baby font.</summary>
        Font FontSmall { get; set; } = new Font("Cascadia", 14, FontStyle.Regular, GraphicsUnit.Point, 0);
        #endregion

        #region Events
        /// <summary>Value changed by user.</summary>
        public event EventHandler CurrentTimeChanged;
        #endregion


        void UpdateTime()
        {


            Invalidate();
        }

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public TimeBar()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            PreviewKeyDown += TimeBar_PreviewKeyDown;
            Load += TimeBar_Load;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeBar_Load(object sender, EventArgs e)
        {
        }
        #endregion

        #region Designer boilerplate
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                _penBorder.Dispose();
                _penMarker.Dispose();
                _brush.Dispose();
                components.Dispose();
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

            // Draw border.
            pe.Graphics.DrawRectangle(_penBorder, 0, 0, Width - _penBorder.Width, Height - _penBorder.Width);

            if (_current < _lengthX)
            {
                pe.Graphics.FillRectangle(_brush,
                    _penBorder.Width,
                    _penBorder.Width,
                    (Width - 2 * _penBorder.Width) * (int)_current.TotalMilliseconds / (int)_lengthX.TotalMilliseconds,
                    Height - 2 * _penBorder.Width);
            }

            // Text.
            using (StringFormat formatLeft = new StringFormat())
            using (StringFormat formatRight = new StringFormat())
            {
                formatLeft.LineAlignment = StringAlignment.Center;
                formatLeft.Alignment = StringAlignment.Near;
                formatRight.LineAlignment = StringAlignment.Center;
                formatRight.Alignment = StringAlignment.Far;

                pe.Graphics.DrawString(FormatTime(_current), FontLarge, Brushes.Black, ClientRectangle, formatLeft);
                pe.Graphics.DrawString(FormatTime(_lengthX), FontSmall, Brushes.Black, ClientRectangle, formatRight);
            }
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
                _current = GetTimeFromMouse(e.X);
                CurrentTimeChanged?.Invoke(this, new EventArgs());
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
            UpdateTime();

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handle dragging.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _current = GetTimeFromMouse(e.X);
            CurrentTimeChanged?.Invoke(this, new EventArgs());
            UpdateTime();
            base.OnMouseDown(e);
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Convert x pos to TimeSpan.
        /// </summary>
        /// <param name="x"></param>
        private TimeSpan GetTimeFromMouse(int x)
        {
            int msec = 0;

            if(_current.TotalMilliseconds < _lengthX.TotalMilliseconds)
            {
                msec = x * (int)_lengthX.TotalMilliseconds / Width;
                msec = MathUtils.Constrain(msec, 0, (int)_lengthX.TotalMilliseconds);
            }

            return new TimeSpan(0, 0, 0, 0, msec);
        }

        /// <summary>
        /// Offset current by msec.
        /// </summary>
        /// <param name="msec"></param>
        void AdjustTime(int msec)
        {
            if (msec > 0)
            {
                _current.Add(new TimeSpan(0, 0, 0, 0, msec));
            }
            else if (msec < 0)
            {
                _current.Subtract(new TimeSpan(0, 0, 0, 0, msec));
            }

            // Sanity checks.
            if (_current > _lengthX)
            {
                _current = _lengthX;
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
            return $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:000}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeBar_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
        #endregion
    }
}
