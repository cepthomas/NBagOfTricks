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
    public class BarSpan
    {
        public int Bar { get; private set; } = 0;
        public int Beat { get; private set; } = 0;
        public int Tick { get; private set; } = 0;

        /// <summary>Only tested with 4.</summary>
        public static int BeatsPerBar { get; set; } = 4;

        /// <summary>Our resolution.</summary>
        public static int TicksPerBeat { get; set; } = 8;

        public static readonly BarSpan BAR_ONE = new BarSpan();

        public int TotalTicks
        {
            get
            {
                int newtick = (Bar * BeatsPerBar * TicksPerBeat) + (Beat * TicksPerBeat) + Tick;
                return newtick;
            }
            set
            {
                Bar = value / BeatsPerBar / TicksPerBeat;
                Beat = (value / TicksPerBeat) % BeatsPerBar;
                Tick = value % TicksPerBeat;
            }
        }

        public void Reset()
        {
            Bar = 0;
            Beat = 0;
            Tick = 0;
        }

        public void Constrain(BarSpan lower, BarSpan upper)
        {
            //TODOC Start = MathUtils.Constrain(Start, 0, Length);
        }

        public override string ToString()
        {
            return $"{Bar + 1}.{Beat + 1}.{Tick + 1:00}";
        }
    }


    public partial class BarBar : UserControl
    {
        #region Enums
        public enum SnapType { Tick, Beat, Bar }
        #endregion

        #region Fields
        ///// <summary>Total length in ticks.</summary>
        //int _length = 0;

        ///// <summary>One marker.</summary>
        //int _start = 0;

        ///// <summary>Other marker.</summary>
        //int _end = 0;

        /// <summary>Current tick.</summary>
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
        ///// <summary>Only tested with 4.</summary>
        //public int BeatsPerBar { get; set; } = 4;

        ///// <summary>Our resolution.</summary>
        //public int TicksPerBeat { get; set; } = 8;

        /// <summary>Oh snap.</summary>
        public SnapType Snap { get; set; } = SnapType.Tick;

        ///// <summary>Total length in ticks. TODOC not changeable?</summary>
        public BarSpan Length { get; set; } = new BarSpan();
        //public int Length { get { return _length; } set { _length = value; Invalidate(); } }

        /// <summary>One marker.</summary>
        public BarSpan Start { get; set; } = new BarSpan();
        //public int Start { get { return _start; } set { _start = value; Invalidate(); } }

        /// <summary>Other marker.</summary>
        public BarSpan End { get; set; } = new BarSpan();
        // public int End { get { return _end; } set { _end = value; Invalidate(); } }

        /// <summary>Where we be now.</summary>
        //public int _current { get; set; } = 0;
        public BarSpan Current { get { return _current; } set { _current = value; Invalidate(); } }

        /// <summary>For styling.</summary>
        public Color ProgressColor { get { return _brush.Color; } set { _brush.Color = value; } }

        /// <summary>Big font.</summary>
        Font FontLarge { get; set; } = new Font("Cascadia", 24, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>Baby font.</summary>
        Font FontSmall { get; set; } = new Font("Cascadia", 14, FontStyle.Regular, GraphicsUnit.Point, 0);
        #endregion

        ///// <summary>
        ///// Check current values.
        ///// </summary>
        ///// <param name="redraw"></param>
        //void UpdateValues(bool redraw)
        //{
        //    if(redraw)
        //    {
        //        Invalidate();
        //    }
        //}

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
            Load += BarBar_Load;
            KeyDown += BarBar_KeyDown;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarBar_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                Start.Reset();
                End.Reset();
                Invalidate();
            }
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
            Start.Constrain(BarSpan.BAR_ONE, Length);
            Start.Constrain(BarSpan.BAR_ONE, End);
            End.Constrain(BarSpan.BAR_ONE, Length);
            End.Constrain(Start, Length);

            if(End == BarSpan.BAR_ONE && Length != BarSpan.BAR_ONE)
            {
                End = Length; //TODOC clone?
            }

            // Draw the bar.
            if (_current.TotalTicks < Length.TotalTicks)
            {
                int len = 0;//_current > End ? Width * End / Length : Width * _current / Length
                int start = 0; //Start

                pe.Graphics.FillRectangle(_brush, start, 0, len, Height);
            }
            // TODOC else????

            // Draw start/end markers.
            if (Start.TotalTicks != 0 || End.TotalTicks != 0)
            {
                int start = 0; //Start
                int end = 0; //End
                pe.Graphics.DrawLine(_pen, start, 0, start, Height);
                pe.Graphics.DrawLine(_pen, end, 0, end, Height);
            }

            // Text.
            pe.Graphics.DrawString(_current.ToString(), FontLarge, Brushes.Black, ClientRectangle, _formatLeft);
            pe.Graphics.DrawString(Length.ToString(), FontSmall, Brushes.Black, ClientRectangle, _formatRight);
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
                _current = DoSnap(GetTickFromMouse(e.X));
                CurrentTimeChanged?.Invoke(this, new EventArgs());
            }
            else if (e.X != _lastXPos)
            {
                var ts = DoSnap(GetTickFromMouse(e.X));
                _toolTip.SetToolTip(this, ts.ToString());
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
            _current = DoSnap(GetTickFromMouse(e.X));
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
        int GetTickFromMouse(int x)
        {
            int tick = 0;

            if(_current.TotalTicks < Length.TotalTicks)
            {
                tick = x * Length.TotalTicks / Width;
                tick = MathUtils.Constrain(tick, 0, Length.TotalTicks);
            }

            return tick;
        }

        /// <summary>
        /// Snap to closest boundary.
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
        BarSpan DoSnap(int tick)
        {
            //int snapped = tick;
            BarSpan bspan = new BarSpan();
            bspan.TotalTicks = tick;

            switch (Snap)
            {
                case SnapType.Bar:
                    {
                        int newbar = bspan.Bar;
                        if (bspan.Beat >= BarSpan.BeatsPerBar / 2)
                        {
                            newbar++;
                        }
                        snapped = TimeToTick(newbar, 0, 0);
                    }
                    break;

                case SnapType.Beat:
                    {
                        int newbar = bspan.Bar;
                        int newbeat = bspan.Beat;
                        if (bspan.Tick >= BarSpan.TicksPerBeat / 2)
                        {
                            newbeat++;
                            if(newbeat >= BarSpan.BeatsPerBar)
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

            return snapped > Length ? Length : snapped;
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
