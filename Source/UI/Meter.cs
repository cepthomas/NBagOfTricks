using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NBagOfTricks.Utils;


namespace NBagOfTricks.UI
{
    /// <summary>Display types.</summary>
    public enum MeterType { Linear, Log, ContinuousLine, ContinuousDots }

    /// <summary>
    /// Implements a rudimentary volume meter. TODO Add damping/vu and/or peak/hold?
    /// </summary>
    public partial class Meter : UserControl
    {
        #region Fields
        /// <summary>Storage.</summary>
        double[] _buff = { };

        /// <summary>Storage.</summary>
        int _buffIndex = 0;

        /// <summary>The pen.</summary>
        readonly Pen _pen = new Pen(Color.Black, 1);

        /// <summary>The brush.</summary>
        readonly SolidBrush _brush = new SolidBrush(Color.White);
        #endregion

        #region Properties
        /// <summary>Optional label.</summary>
        public string Label { get; set; } = "";

        /// <summary>For styling.</summary>
        public Color DrawColor { get { return _brush.Color; } set { _brush.Color = value; } }

        /// <summary>How the meter responds.</summary>
        public MeterType MeterType { get; set; } = MeterType.Linear;

        /// <summary>Minimum value. If Log type, this is in db - usually -60;</summary>
        public double Minimum { get; set; } = 0;

        /// <summary>Maximum value. If Log type, this is in db - usually +18.</summary>
        public double Maximum { get; set; } = 100;

        /// <summary>Meter orientation.</summary>
        public Orientation Orientation { get; set; } = Orientation.Horizontal;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Basic volume meter.
        /// </summary>
        public Meter()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Name = "Meter";
            Load += Meter_Load;
        }

        /// <summary>
        /// Init stuff.
        /// </summary>
        private void Meter_Load(object sender, EventArgs e)
        {
        }
        #endregion

        #region Designer boilerplate
        /// <summary>
        /// Required designer variable.
        /// </summary>
        readonly System.ComponentModel.IContainer components = new System.ComponentModel.Container();

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

        #region Public functions
        /// <summary>
        /// Add a new data point. If Log, this will convert for you.
        /// </summary>
        /// <param name="val"></param>
        public void AddValue(double val)
        {
            // Sometimes when you minimize, samples can be set to 0.
            if (_buff.Length != 0)
            {
                switch (MeterType)
                {
                    case MeterType.Log:
                        double db = MathUtils.Constrain(20 * Math.Log10(val), Minimum, Maximum);
                        _buff[0] = db;
                        break;

                    case MeterType.Linear:
                        double lval = MathUtils.Constrain(val, Minimum, Maximum);
                        _buff[0] = lval;
                        break;

                    case MeterType.ContinuousLine:
                    case MeterType.ContinuousDots:
                        // Bump ring index.
                        _buffIndex++;
                        _buffIndex %= _buff.Length;
                        _buff[_buffIndex] = MathUtils.Constrain(val, Minimum, Maximum);
                        break;
                }

                Invalidate();
            }
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Paints the volume meter.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.Clear(BackColor);
            // Border
            pe.Graphics.DrawRectangle(_pen, 0, 0, Width - _pen.Width, Height - _pen.Width);

            // Draw data.
            Rectangle drawArea = Rectangle.Inflate(ClientRectangle, -(int)_pen.Width, -(int)_pen.Width);

            switch (MeterType)
            {
                case MeterType.Log:
                case MeterType.Linear:
                    double percent = _buff.Length > 0 ? (_buff[0] - Minimum) / (Maximum - Minimum) : 0;

                    if (Orientation == Orientation.Horizontal)
                    {
                        int w = (int)(drawArea.Width * percent);
                        int h = drawArea.Height;
                        pe.Graphics.FillRectangle(_brush, _pen.Width, _pen.Width, w, h);
                    }
                    else
                    {
                        int w = drawArea.Width;
                        int h = (int)(drawArea.Height * percent);
                        pe.Graphics.FillRectangle(_brush, _pen.Width, Height - _pen.Width - h, w, h);
                    }
                    break;

                case MeterType.ContinuousLine:
                case MeterType.ContinuousDots:
                    for (int i = 0; i < _buff.Length; i++)
                    {
                        int index = _buffIndex - i;
                        index = index < 0 ? index + _buff.Length : index;

                        double val = _buff[index];

                        // Draw data point.
                        double x = i + _pen.Width;
                        double y = MathUtils.Map(val, Minimum, Maximum, drawArea.Height - _pen.Width, _pen.Width);

                        if(MeterType == MeterType.ContinuousLine)
                        {
                            pe.Graphics.DrawLine(_pen, (float)x, (float)y, (float)x, drawArea.Height - 2 * _pen.Width);
                        }
                        else
                        {
                            pe.Graphics.FillRectangle(_brush, (float)x, (float)y, 2, 2);
                        }
                    }
                    break;
            }

            if (Label.Length > 0)
            {
                using (StringFormat format = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center })
                {
                    Rectangle r = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height / 2);
                    pe.Graphics.DrawString(Label, Font, Brushes.Black, r, format);
                }
            }
        }

        /// <summary>
        /// Update drawing area.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            int buffSize = (int)(Width - 2 * _pen.Width);
            _buff = new double[buffSize];
            for(int i = 0; i < buffSize; i++)
            {
                _buff[i] = Minimum;
            }
            _buffIndex = 0;
            base.OnResize(e);
            Invalidate();
        }
        #endregion
    }
}
