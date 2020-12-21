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
    #region Enums
    public enum SnapType { Tick, Beat, Bar }
    #endregion

    /// <summary>Sort of like TimeSpan.</summary>
    public struct BarSpan
    {
        #region Fields
        /// <summary>For hashing.</summary>
        int _id;

        /// <summary>Increment for unique value.</summary>
        static int _all_ids = 1;
        #endregion

        #region Properties
        /// <summary>The core.</summary>
        public int TotalTicks { get; private set; }

        /// <summary>Global - set before using. Only tested with 4.</summary>
        public static int BeatsPerBar { get; set; } = 4;

        /// <summary>Global - set before using. Our resolution.</summary>
        public static int TicksPerBeat { get; set; } = 8;

        /// <summary>Global - set before using.</summary>
        public static SnapType Snap { get; set; } = SnapType.Tick;

        /// <summary>A useful constant.</summary>
        public static readonly BarSpan BAR_ONE = new BarSpan();

        /// <summary>The bar.</summary>
        public int Bar { get { return TotalTicks / BeatsPerBar / TicksPerBeat; } }

        /// <summary>The beat.</summary>
        public int Beat { get { return (TotalTicks / TicksPerBeat) % BeatsPerBar; } }

        /// <summary>The tick.</summary>
        public int Tick { get { return TotalTicks % TicksPerBeat; } }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor from args.
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="beat"></param>
        /// <param name="tick"></param>
        public BarSpan(int bar, int beat, int tick)
        {
            TotalTicks = (bar * BeatsPerBar * TicksPerBeat) + (beat * TicksPerBeat) + tick;
            _id = _all_ids++;
        }

        /// <summary>
        /// Constructor from args.
        /// </summary>
        /// <param name="ticks"></param>
        BarSpan(int ticks)
        {
            TotalTicks = ticks;
            _id = _all_ids++;
        }
        #endregion

        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            TotalTicks = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public void Constrain(BarSpan lower, BarSpan upper)
        {
            TotalTicks = MathUtils.Constrain(TotalTicks, lower.TotalTicks, upper.TotalTicks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        public void Increment(int num)
        {
            TotalTicks += num;
            if(TotalTicks < 0)
            {
                TotalTicks = 0;
            }
        }

        /// <summary>
        /// Snap to closest boundary.
        /// </summary>
        /// <param name="tick"></param>
        public void DoSnap(int tick)
        {
            //int snapped = tick;
            BarSpan bspan = new BarSpan();
            bspan.TotalTicks = tick;
            int newbar = bspan.Bar;
            int newbeat = bspan.Beat;

            switch (Snap)
            {
                case SnapType.Bar:
                    {
                        if (newbeat >= BeatsPerBar / 2)
                        {
                            newbar++;
                        }
                    }
                    TotalTicks = (newbar * BeatsPerBar * TicksPerBeat);
                    break;

                case SnapType.Beat:
                    {
                        if (bspan.Tick >= TicksPerBeat / 2)
                        {
                            newbeat++;
                            if (newbeat >= BeatsPerBar)
                            {
                                newbar++;
                                newbeat = 0;
                            }
                        }
                        TotalTicks = (newbar * BeatsPerBar * TicksPerBeat) + (newbeat * TicksPerBeat);
                    }
                    break;

                case SnapType.Tick:
                    // Don't change it.
                    TotalTicks = tick;
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Bar + 1}.{Beat + 1}.{Tick + 1:00}";
        }
        #endregion

        #region Standard comparable stuff
        public override bool Equals(object obj)
        {
            return obj is BarSpan && ((BarSpan)obj).TotalTicks == TotalTicks;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static bool operator ==(BarSpan a, BarSpan b)
        {
            return a.TotalTicks == b.TotalTicks;
        }

        public static bool operator !=(BarSpan a, BarSpan b)
        {
            return !(a == b);
        }

        public static BarSpan operator +(BarSpan a, BarSpan b)
        {
            return new BarSpan(a.TotalTicks + b.TotalTicks);
        }

        public static BarSpan operator -(BarSpan a, BarSpan b)
        {
            return new BarSpan(a.TotalTicks - b.TotalTicks);
        }

        public static bool operator <(BarSpan a, BarSpan b)
        {
            return a.TotalTicks < b.TotalTicks;
        }

        public static bool operator >(BarSpan a, BarSpan b)
        {
            return a.TotalTicks > b.TotalTicks;
        }

        public static bool operator <=(BarSpan a, BarSpan b)
        {
            return a.TotalTicks <= b.TotalTicks;
        }

        public static bool operator >=(BarSpan a, BarSpan b)
        {
            return a.TotalTicks >= b.TotalTicks;
        }
        #endregion
    }

    /// <summary>The control.</summary>
    public partial class BarBar : UserControl
    {
        #region Fields
        /// <summary>Total length.</summary>
        BarSpan _length = new BarSpan();

        /// <summary>One marker.</summary>
        BarSpan _start = new BarSpan();

        /// <summary>Other marker.</summary>
        BarSpan _end = new BarSpan();

        /// <summary>Current.</summary>
        BarSpan _current = new BarSpan();

        /// <summary>For tracking mouse moves.</summary>
        int _lastXPos = 0;

        /// <summary>Tooltip for mousing.</summary>
        readonly ToolTip _toolTip = new ToolTip();

        /// <summary>The brush.</summary>
        readonly SolidBrush _brush = new SolidBrush(Color.White);

        /// <summary>The pen.</summary>
        readonly Pen _pen = new Pen(Color.Black, 1);

        /// <summary>For drawing text.</summary>
        StringFormat _formatLeft = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near };

        /// <summary>For drawing text.</summary>
        StringFormat _formatRight = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Far };
        #endregion

        #region Properties
        /// <summary>Total length.</summary>
        public BarSpan Length { get { return _length; } set { _length = value; Invalidate(); } }

        /// <summary>One marker.</summary>
        public BarSpan Start { get { return _start; } set { _start = value; Invalidate(); } }

        /// <summary>Other marker.</summary>
        public BarSpan End { get { return _end; } set { _end = value; Invalidate(); } }

        /// <summary>Where we be now.</summary>
        public BarSpan Current { get { return _current; } set { _current = value; Invalidate(); } }

        /// <summary>For styling.</summary>
        public Color ProgressColor { get { return _brush.Color; } set { _brush.Color = value; } }

        /// <summary>Big font.</summary>
        public Font FontLarge { get; set; } = new Font("Cascadia", 24, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>Baby font.</summary>
        public Font FontSmall { get; set; } = new Font("Cascadia", 14, FontStyle.Regular, GraphicsUnit.Point, 0);
        #endregion

        #region Events
        /// <summary>Value changed by user.</summary>
        public event EventHandler CurrentTimeChanged;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        public BarBar()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
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
                _brush.Dispose();
                _pen.Dispose();
                _formatLeft.Dispose();
                _formatRight.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Draw the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Setup.
            pe.Graphics.Clear(BackColor);

            // Validate times.
            _start.Constrain(BarSpan.BAR_ONE, _length);
            _start.Constrain(BarSpan.BAR_ONE, _end);
            _end.Constrain(BarSpan.BAR_ONE, _length);
            _end.Constrain(_start, _length);

            if(_end == BarSpan.BAR_ONE && _length != BarSpan.BAR_ONE)
            {
                _end = _length;
            }

            // Draw the bar.
            if (_current < _length)
            {
                int len = _current > _end ? Scale(_end) : Scale(_current);
                int start = Scale(_current);

                pe.Graphics.FillRectangle(_brush, start, 0, len, Height);
            }
            // TODOC else????

            // Draw start/end markers.
            if (_start != BarSpan.BAR_ONE || _end != BarSpan.BAR_ONE)
            {
                int start = Scale(_start);
                int end = Scale(_end);
                pe.Graphics.DrawLine(_pen, start, 0, start, Height);
                pe.Graphics.DrawLine(_pen, end, 0, end, Height);
            }

            // Text.
            pe.Graphics.DrawString(_current.ToString(), FontLarge, Brushes.Black, ClientRectangle, _formatLeft);
            pe.Graphics.DrawString(_length.ToString(), FontSmall, Brushes.Black, ClientRectangle, _formatRight);
        }
        #endregion

        #region UI handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                // Reset.
                _start.Reset();
                _end.Reset();
                Invalidate();
            }
        }

        /// <summary>
        /// Handle mouse position changes.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _current.DoSnap(GetTickFromMouse(e.X));
                CurrentTimeChanged?.Invoke(this, new EventArgs());
            }
            else if (e.X != _lastXPos)
            {
                _current.DoSnap(GetTickFromMouse(e.X));
                _toolTip.SetToolTip(this, _current.ToString());
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
            _current.DoSnap(GetTickFromMouse(e.X));
            CurrentTimeChanged?.Invoke(this, new EventArgs());

            Invalidate();
            base.OnMouseDown(e);
        }
        #endregion

        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        public void IncrementCurrent(int num)
        {
            _current.Increment(num);
            if (_current < BarSpan.BAR_ONE)
            {
                _current = BarSpan.BAR_ONE;
            }
            if (_current >= _length)
            {
                _current = _length;
            }

            Invalidate();
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Convert x pos to tick.
        /// </summary>
        /// <param name="x"></param>
        int GetTickFromMouse(int x)
        {
            int tick = 0;

            if(_current < _length)
            {
                tick = x * _length.TotalTicks / Width;
                tick = MathUtils.Constrain(tick, 0, _length.TotalTicks);
            }

            return tick;
        }

        /// <summary>
        /// Map from time to UI pixels.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int Scale(BarSpan val)
        {
            return val.TotalTicks * Width / _length.TotalTicks;
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
