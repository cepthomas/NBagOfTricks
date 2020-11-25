using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using NBagOfTricks.Utils;


namespace NBagOfTricks.UI
{
    public partial class BarBar : UserControl
    {
        #region Enums
        public enum SnapType { None, Bar, Beat }
        #endregion

        #region Fields
        /// <summary>Total length.</summary>
        int _lengthTicks = 0;

        /// <summary>Current time/position.</summary>
        int _currentTick = 0;

        /// <summary>For tracking mouse moves.</summary>
        int _lastXPos = 0;

        /// <summary>Tooltip for mousing.</summary>
        readonly ToolTip toolTip = new ToolTip();

        /// <summary>The pen.</summary>
        readonly Pen _pen = new Pen(Color.Black, UiDefs.BORDER_WIDTH);

        /// <summary>The brush.</summary>
        readonly SolidBrush _brush = new SolidBrush(Color.White);
        #endregion

        #region Properties
        /// <summary>Current bar from 0 to N.</summary>
        public int BeatsPerBar { get; set; } = 4;

        /// <summary>Our ppq aka resolution.</summary>
        public int TicksPerBeat { get; set; } = 8;

        /// <summary>Oh snap.</summary>
        public SnapType Snap { get; set; } = SnapType.None;

        /// <summary>Where we be now.</summary>
        public int CurrentTick { get { return _currentTick; } set { _currentTick = value; Invalidate(); } }

        /// <summary> </summary>
        public (int bar, int beat, int tick) CurrentTime { get { return TickToTime(_currentTick); } }

        /// <summary>Where we be going.</summary>
        public int Length { get { return _lengthTicks; } set { _lengthTicks = value; Invalidate(); } }

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

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public BarBar()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Load += BarBar_Load;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BarBar_Load(object sender, EventArgs e)
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
            pe.Graphics.DrawRectangle(_pen, 0, 0, Width - UiDefs.BORDER_WIDTH, Height - UiDefs.BORDER_WIDTH);

            if (_currentTick < _lengthTicks)
            {
                pe.Graphics.FillRectangle(_brush,
                    UiDefs.BORDER_WIDTH,
                    UiDefs.BORDER_WIDTH,
                    (Width - 2 * UiDefs.BORDER_WIDTH) * _currentTick / _lengthTicks,
                    Height - 2 * UiDefs.BORDER_WIDTH);
            }

            // Text.
            using (StringFormat formatLeft = new StringFormat())
            using (StringFormat formatRight = new StringFormat())
            {
                formatLeft.LineAlignment = StringAlignment.Center;
                formatLeft.Alignment = StringAlignment.Near;
                formatRight.LineAlignment = StringAlignment.Center;
                formatRight.Alignment = StringAlignment.Far;

                pe.Graphics.DrawString(FormatTime(_currentTick), FontLarge, Brushes.Black, ClientRectangle, formatLeft);
                pe.Graphics.DrawString(FormatTime(_lengthTicks), FontSmall, Brushes.Black, ClientRectangle, formatRight);
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
                _currentTick = DoSnap(GetTickFromMouse(e.X));
                CurrentTimeChanged?.Invoke(this, new EventArgs());
            }
            else if (e.X != _lastXPos)
            {
                int ts = DoSnap(GetTickFromMouse(e.X));
                toolTip.SetToolTip(this, FormatTime(ts));
                _lastXPos = e.X;
            }
            Invalidate();
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handle dragging.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _currentTick = DoSnap(GetTickFromMouse(e.X));
            CurrentTimeChanged?.Invoke(this, new EventArgs());
            Invalidate();
            base.OnMouseDown(e);
        }
        #endregion

        #region Public functions
        //void Advance()
        //{
        //    //int _lengthTicks = 0;
        //    //int _currentTicks = 0;
        //}
        #endregion

        #region Private functions
        /// <summary>
        /// Convert x pos to tick.
        /// </summary>
        /// <param name="x"></param>
        private int GetTickFromMouse(int x)
        {
            int tick = 0;

            if(_currentTick < _lengthTicks)
            {
                tick = x * _lengthTicks / Width;
                tick = MathUtils.Constrain(tick, 0, _lengthTicks);
            }

            return tick;
        }

        /// <summary>
        /// Pretty print a time.
        /// </summary>
        /// <param name="tick"></param>
        string FormatTime(int tick)
        {
            var t = TickToTime(tick);
            return $"{t.bar+1}.{t.beat+1}.{t.tick:00}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        (int bar, int beat, int tick) TickToTime(int ticks)
        {
            int bar = ticks / BeatsPerBar / TicksPerBeat;
            int beat = (ticks / TicksPerBeat) % BeatsPerBar;
            int tick = ticks % (BeatsPerBar * TicksPerBeat);

            return (bar, beat, tick);
        }

        /// <summary>
        /// Snap to closest boundary.
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
        int DoSnap(int tick) //TODOC
        {
            int snapped = tick;

            switch (Snap)
            {
                case SnapType.Bar:

                    break;

                case SnapType.Beat:

                    break;

                case SnapType.None:
                    // Don't change it.
                    break;
            }

            return snapped;
        }
        #endregion
    }
}
