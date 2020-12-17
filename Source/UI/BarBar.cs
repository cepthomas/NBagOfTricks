using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NBagOfTricks.Utils;


namespace NBagOfTricks.UI
{
    public partial class BarBar : UserControl
    {
        #region Enums
        public enum SnapType { Tick, Beat, Bar }
        #endregion

        #region Fields
        /// <summary>Total length.</summary>
        int _lengthTicks = 0;

        /// <summary>One marker.</summary>
        int _marker1 = 0;

        /// <summary>Other marker.</summary>
        int _marker2 = 0;

        /// <summary>Current time/position.</summary>
        int _currentTick = 0;

        /// <summary>For tracking mouse moves.</summary>
        int _lastXPos = 0;

        /// <summary>Tooltip for mousing.</summary>
        readonly ToolTip _toolTip = new ToolTip();

        /// <summary>The pen.</summary>
        readonly Pen _penBorder = new Pen(Color.Black, 1);

        /// <summary>The marker pen.</summary>
        readonly Pen _penMarker = new Pen(Color.Black, 1);

        /// <summary>The brush.</summary>
        readonly SolidBrush _brush = new SolidBrush(Color.White);
        #endregion

        #region Properties
        /// <summary>Only tested with 4.</summary>
        public int BeatsPerBar { get; set; } = 4;

        /// <summary>Our ppq aka resolution.</summary>
        public int TicksPerBeat { get; set; } = 8;

        /// <summary>Oh snap.</summary>
        public SnapType Snap { get; set; } = SnapType.Tick;

        /// <summary>Total length in ticks.</summary>
        public int Length { get { return _lengthTicks; } set { _lengthTicks = value; UpdateBar(); } }

        /// <summary>One marker.</summary>
        public int Marker1 { get { return _marker1; } set { _marker1 = value; UpdateBar(); } }

        /// <summary>Other marker.</summary>
        public int Marker2 { get { return _marker2; } set { _marker2 = value; UpdateBar(); } }

        /// <summary>Where we be now.</summary>
        public int CurrentTick { get { return _currentTick; } set { _currentTick = value; UpdateBar(); } }

        /// <summary>For styling.</summary>
        public Color ProgressColor { get { return _brush.Color; } set { _brush.Color = value; } }

        /// <summary>Big font.</summary>
        Font FontLarge { get; set; } = new Font("Cascadia", 24, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>Baby font.</summary>
        Font FontSmall { get; set; } = new Font("Cascadia", 14, FontStyle.Regular, GraphicsUnit.Point, 0);
        #endregion


        void UpdateBar()
        {


            Invalidate();
        }




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

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _toolTip.Dispose();
                _penBorder.Dispose();
                _penMarker.Dispose();
                _brush.Dispose();
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

            if (_currentTick < _lengthTicks)
            {
                pe.Graphics.FillRectangle(_brush,
                    _penBorder.Width,
                    _penBorder.Width,
                    (Width - 2 * _penBorder.Width) * _currentTick / _lengthTicks,
                    Height - 2 * _penBorder.Width);
            }

            // Text.
            using (StringFormat formatLeft = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near })
            using (StringFormat formatRight = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Far })
            {
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
                //CurrentTimeChanged?.Invoke(this, new EventArgs());
            }
            else if (e.X != _lastXPos)
            {
                int ts = DoSnap(GetTickFromMouse(e.X));
                _toolTip.SetToolTip(this, FormatTime(ts));
                _lastXPos = e.X;
            }
            UpdateBar();
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handle dragging.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _currentTick = DoSnap(GetTickFromMouse(e.X));
            CurrentTimeChanged?.Invoke(this, new EventArgs());
            UpdateBar();
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
        int GetTickFromMouse(int x)
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
            return $"{t.bar+1}.{t.beat+1}.{t.tick+1:00}";
        }

        /// <summary>
        /// Conversion.
        /// </summary>
        /// <param name="itick"></param>
        /// <returns>0-based</returns>
        (int bar, int beat, int tick) TickToTime(int itick)
        {
            int bar = itick / BeatsPerBar / TicksPerBeat;
            int beat = (itick / TicksPerBeat) % BeatsPerBar;
            int tick = itick % TicksPerBeat;

            return (bar, beat, tick);
        }

        /// <summary>
        /// Conversion.
        /// </summary>
        /// <param name="bar">0-based</param>
        /// <param name="beat">0-based</param>
        /// <param name="tick">0-based</param>
        /// <returns></returns>
        int TimeToTick(int bar, int beat, int tick)
        {
            int newtick = (bar * BeatsPerBar * TicksPerBeat) + (beat * TicksPerBeat) + tick;
            return newtick;
        }

        /// <summary>
        /// Snap to closest boundary.
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
        int DoSnap(int tick)
        {
            int snapped = tick;

            var tm = TickToTime(tick);

            switch (Snap)
            {
                case SnapType.Bar:
                    {
                        int newbar = tm.bar;
                        if (tm.beat >= BeatsPerBar / 2)
                        {
                            newbar++;
                        }
                        snapped = TimeToTick(newbar, 0, 0);
                    }
                    break;

                case SnapType.Beat:
                    {
                        int newbar = tm.bar;
                        int newbeat = tm.beat;
                        if (tm.tick >= TicksPerBeat / 2)
                        {
                            newbeat++;
                            if(newbeat >= BeatsPerBar)
                            {
                                newbar++;
                                newbeat = 0;
                            }
                        }
                        snapped = TimeToTick(newbar, newbeat, 0);
                    }
                    break;

                case SnapType.Tick:
                    // Don't change it.
                    break;
            }

            return snapped > _lengthTicks ? _lengthTicks : snapped;
        }
        #endregion

        //public void Test()
        //{
        //    Snap = SnapType.Beat;

        //    List<string> res = new List<string>();
        //    res.Add("tick,snapped_tick,tm.bar,tm.beat,tm.tick");

        //    for (int tick = 0; tick < 1000; tick++)
        //    {
        //        if(tick == 23)
        //        {
        //        }
        //        int newtick = DoSnap(tick);
        //        var tm = TickToTime(newtick);

        //        string s = $"{tick},{newtick},{tm.bar},{tm.beat},{tm.tick}";
        //        res.Add(s);
        //    }
        //    File.WriteAllText("bars.csv", string.Join(Environment.NewLine, res));
        //}
    }
}
