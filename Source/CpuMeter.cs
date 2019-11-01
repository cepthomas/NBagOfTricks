﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace NBagOfTricks
{
    public partial class CpuMeter : UserControl
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        PerformanceCounter[] _pc = null;

        /// <summary>
        /// 
        /// </summary>
        Timer _timer = new Timer();

        /// <summary>
        /// 
        /// </summary>
        int _min = 0;

        /// <summary>
        /// 
        /// </summary>
        int _max = 100;

        /// <summary>
        /// Storage.
        /// </summary>
        double[][] _buff = null;

        /// <summary>
        /// Storage.
        /// </summary>
        double[] _sum = null;

        /// <summary>
        /// Storage.
        /// </summary>
        int _buffIndex = 0;

        /// <summary>
        /// A number.
        /// </summary>
        const int BORDER_WIDTH = 1;
        #endregion

        #region Properties
        /// <summary>
        /// Default is 500 msec. Change if you like.
        /// </summary>
        public int UpdateFreq { set { _timer.Interval = value; } }

        /// <summary>
        /// For styling.
        /// </summary>
        public Color ControlColor { get; set; } = Color.Orange;
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

            int cores = 0;
            int physicalProcessors = 0;
            int logicalProcessors = 0;

            using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_Processor"))
            {
                var items = searcher.Get();
                foreach (var item in items)
                {
                    cores = int.Parse(item["NumberOfCores"].ToString());
                }
            }

            using (var searcher = new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                var items = searcher.Get();
                foreach (var item in items)
                {
                    physicalProcessors = int.Parse(item["NumberOfProcessors"].ToString());
                    logicalProcessors = int.Parse(item["NumberOfLogicalProcessors"].ToString());
                }
            }

            _pc = new PerformanceCounter[logicalProcessors];
            _buff = new double[logicalProcessors][];

            for (int i = 0; i < logicalProcessors; i++)
            {
                var pc = new PerformanceCounter("Processor", "% Processor Time", i.ToString());
                _pc[i] = pc;
            }
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
            // Setup.
            pe.Graphics.Clear(BackColor);
            Brush brush = new SolidBrush(ControlColor);
            Pen pen = new Pen(ControlColor);

            // Draw border.
            int bw = BORDER_WIDTH;
            Pen penBorder = new Pen(Color.Black, bw);
            pe.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            // Draw data.
            Rectangle drawArea = Rectangle.Inflate(ClientRectangle, -bw, -bw);

            for (int i = 0; i < _sum.Length; i++)
            {
                int index = _buffIndex - i;
                index = index < 0 ? index + _sum.Length : index;

                double val = _sum[index];

                // Draw data point.
                double x = i + bw;
                double y = MathUtils.Map(val, _min, _max, Height - 2 * bw, bw);

                pe.Graphics.DrawLine(pen, (float)x, (float)y, (float)x, Height - 2 * bw);
                // or: pe.Graphics.FillRectangle(brush, (float)x, (float)y, 2, 2);
            }

            StringFormat format = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
            Rectangle r = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height / 2);
            pe.Graphics.DrawString("CPU Meter", Font, Brushes.Black, r, format);
        }

        /// <summary>
        /// Update drawing area.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            int size = Width - 2 * BORDER_WIDTH;
            for (int i = 0; i < _buff.Count(); i++)
            {
                _buff[i] = new double[size];
            }
            _sum = new double[size];

            _buffIndex = 0;
            base.OnResize(e);
            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            _sum[_buffIndex] = 0;

            for (int i = 0; i < _pc.Count(); i++)
            {
                float val = _pc[i].NextValue();
                _buff[i][_buffIndex] = val;
                _sum[_buffIndex] += val;
            }

            _buffIndex++;
            if(_buffIndex >= _sum.Count())
            {
                _buffIndex = 0;
            }

            Invalidate();
        }
        #endregion
   }
}
