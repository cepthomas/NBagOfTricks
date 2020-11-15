using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using NBagOfTricks.Utils;


namespace NBagOfTricks.UI
{
    public partial class CpuMeter : UserControl
    {
        #region Fields
        /// <summary>Total usage.</summary>
        PerformanceCounter _cpuPerf = null;

        /// <summary>Logical processes.</summary>
        PerformanceCounter[] _processesPerf = null;

        /// <summary> </summary>
        bool _inited = false;

        /// <summary> </summary>
        Timer _timer = new Timer();

        /// <summary> </summary>
        int _min = 0;

        /// <summary> </summary>
        int _max = 100;

        /// <summary>Storage.</summary>
        double[][] _processesBuffs = null;

        /// <summary>Storage.</summary>
        double[] _cpuBuff = null;

        /// <summary>Storage.</summary>
        int _buffIndex = 0;

        /// <summary>CPU info.</summary>
        int _cores = 0;

        /// <summary>CPU info.</summary>
        int _physicalProcessors = 0;

        /// <summary>CPU info.</summary>
        int _logicalProcessors = 0;

        /// <summary>The pen.</summary>
        Pen _pen = new Pen(Color.Black, UiDefs.BORDER_WIDTH);
        #endregion

        #region Properties
        /// <summary>User can change.</summary>
        public string Label { get; set; } = "cpu";

        /// <summary> </summary>
        public bool Enable { get; set; } = false;

        /// <summary>Default is 500 msec. Change if you like.</summary>
        public int UpdateFreq { set { _timer.Interval = value; } }

        /// <summary>For styling.</summary>
        public Color DrawColor { get { return _pen.Color; } set { _pen.Color = value; } }
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

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public CpuMeter()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Name = "CpuMeter";
            Load += CpuMeter_Load;
        }

        /// <summary>
        /// Initialize everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CpuMeter_Load(object sender, EventArgs e)
        {
            _timer.Tick += Timer_Tick;
            _timer.Interval = 500;
            _timer.Start();
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Paints the volume meter.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.Clear(BackColor);

            pe.Graphics.DrawRectangle(Pens.Black, 0, 0, Width - UiDefs.BORDER_WIDTH, Height - UiDefs.BORDER_WIDTH);

            // Draw data.
            // TODO for each process?
            // The Processor (% Processor Time) counter will be out of 100 and will give the total usage across all
            // processors /cores/etc in the computer. However, the Processor (% Process Time) is scaled by the number
            // of logical processors. To get average usage across a computer, divide the result by Environment.ProcessorCount.
            if(_cpuBuff != null)
            {
                for (int i = 0; i < _cpuBuff.Length; i++)
                {
                    int index = _buffIndex - i;
                    index = index < 0 ? index + _cpuBuff.Length : index;

                    double val = _cpuBuff[index];

                    // Draw data point.
                    double x = i + UiDefs.BORDER_WIDTH;
                    double y = MathUtils.Map(val, _min, _max, Height - 2 * UiDefs.BORDER_WIDTH, UiDefs.BORDER_WIDTH);

                    pe.Graphics.DrawLine(_pen, (float)x, (float)y, (float)x, Height - 2 * UiDefs.BORDER_WIDTH);
                    // or: pe.Graphics.FillRectangle(brush, (float)x, (float)y, 2, 2);
                }
            }

            using (StringFormat format = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center })
            {
                Rectangle r = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height / 2);
                pe.Graphics.DrawString(Label, Font, Brushes.Black, r, format);
            }
        }

        /// <summary>
        /// Update drawing area.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            if(_inited)
            {
                SetBuffs();
            }

            base.OnResize(e);
            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        void SetBuffs()
        {
            int size = Width - 2 * UiDefs.BORDER_WIDTH;
            for (int i = 0; i < _processesBuffs.Count(); i++)
            {
                _processesBuffs[i] = new double[size];
            }

            _cpuBuff = new double[size];

            _buffIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if(Enable)
            {
                if (_cpuPerf == null)
                {
                    InitPerf();
                }
                else
                {
                    _cpuBuff[_buffIndex] = 0;

                    for (int i = 0; i < _processesPerf.Count(); i++)
                    {
                        float val = _processesPerf[i].NextValue();
                        _processesBuffs[i][_buffIndex] = val;
                    }

                    _cpuBuff[_buffIndex] = _cpuPerf.NextValue();

                    _buffIndex++;
                    if (_buffIndex >= _cpuBuff.Count())
                    {
                        _buffIndex = 0;
                    }

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Defer init as they are slow processes. TODO Init on another thread?
        /// </summary>
        void InitPerf()
        {
            using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_Processor"))
            {
                var items = searcher.Get();
                foreach (var item in items)
                {
                    _cores = int.Parse(item["NumberOfCores"].ToString());
                }
            }

            using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                var items = searcher.Get();
                foreach (var item in items)
                {
                    _physicalProcessors = int.Parse(item["NumberOfProcessors"].ToString());
                    _logicalProcessors = int.Parse(item["NumberOfLogicalProcessors"].ToString());
                }
            }

            _processesPerf = new PerformanceCounter[_logicalProcessors];
            _processesBuffs = new double[_logicalProcessors][];

            for (int i = 0; i < _logicalProcessors; i++)
            {
                var pc = new PerformanceCounter("Processor", "% Processor Time", i.ToString());
                _processesPerf[i] = pc;
            }

            _cpuPerf = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            SetBuffs();

            _inited = true;
        }
        #endregion
    }
}
