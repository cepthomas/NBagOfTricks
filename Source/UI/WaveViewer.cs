using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NBagOfTricks.Utils;
using System.IO;

namespace NBagOfTricks.UI
{
    public partial class WaveViewer : UserControl
    {
        #region Fields
        /// <summary>From client.</summary>
        float[] _rawVals = null;

        /// <summary>Maximum value of _rawVals (+-).</summary>
        float _rawMax = 1.0f;

        /// <summary>Storage for display.</summary>
        float[] _buff = null;

        /// <summary>The border pen.</summary>
        Pen _penBorder = new Pen(Color.Black, 1);

        /// <summary>The other pen.</summary>
        Pen _penDraw = new Pen(Color.Black, 1);

        /// <summary>For drawing text.</summary>
        Font _textFont = new Font("Cascadia", 14, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>For drawing text.</summary>
        StringFormat _format = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
        #endregion

        #region Properties
        /// <summary>For styling.</summary>
        public Color DrawColor { get { return _penDraw.Color; } set { _penDraw.Color = value; } }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public WaveViewer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Load += WaveViewer_Load;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WaveViewer_Load(object sender, EventArgs e)
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
                _penBorder.Dispose();
                _penDraw.Dispose();
                _textFont.Dispose();
                _format.Dispose();
           }
           base.Dispose(disposing);
        }
        #endregion

        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="max"></param>
        public void Init(float[] vals, float max)
        {
            Dump(vals, "raw.csv");
            _rawVals = vals;
            _rawMax = max;

            Rescale();
            Dump(_buff, "buff.csv");
            Invalidate();
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Paints the waveform.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Setup.
            pe.Graphics.Clear(BackColor);

            // Draw border.
            pe.Graphics.DrawRectangle(_penBorder, 0, 0, Width - _penBorder.Width, Height - _penBorder.Width);

            if (_buff == null)
            {
                pe.Graphics.DrawString("No data", _textFont, Brushes.Red, ClientRectangle, _format);
            }
            else
            {
                for (int i = 0; i < _buff.Length; i++)
                {
                    double val = _buff[i];

                    // Draw data point.
                    double x = i + _penBorder.Width;

                    // Line from val to 0
                    double y1 = MathUtils.Map(val, -_rawMax, _rawMax, Height - 2 * _penBorder.Width, _penBorder.Width);
                    double y2 = Height / 2;
                    pe.Graphics.DrawLine(_penDraw, (float)x, (float)y1, (float)x, (float)y2);

                    // Line from +val to -val
                    //double y1 = MathUtils.Map(val, -_rawMax, _rawMax, Height - 2 * _penBorder.Width, _penBorder.Width);
                    //double y2 = MathUtils.Map(val, -_rawMax, _rawMax, _penBorder.Width, Height - 2 * _penBorder.Width);
                    //pe.Graphics.DrawLine(_penDraw, (float)x, (float)y1, (float)x, (float)y2);

                    // Simple dot
                    //pe.Graphics.DrawRectangle(_penDraw, (float)x, (float)y1, 1, 1);
                }
            }
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Scale raw values to fit in available space.
        /// </summary>
        void Rescale()
        {
            if(_rawVals == null)
            {
                _buff = null;
            }
            else
            {
                int fitWidth = (int)(Width - (2 * _penBorder.Width));
                _buff = new float[fitWidth];
                // Bin to fit UI x axis.
                int smplPerPixel = _rawVals.Length / fitWidth;

                int r = 0; // index into raw
                for (int i = 0; i < fitWidth; i++)
                {
                    // Find the largest value in the bin.
                    double max = 0;
                    for (int d = 0; d < smplPerPixel; d++)
                    {
                        if (Math.Abs(_rawVals[r]) > Math.Abs(max))
                        {
                            max = _rawVals[r];
                        }
                        r++;
                    }
                    _buff[i] = (float)max;

                    //// Find the absolute largest value in the bin.
                    //double max = 0;
                    //for (int d = 0; d < smplPerPixel; d++)
                    //{
                    //    max = Math.Max(max, Math.Abs(_rawVals[r]));
                    //    r++;
                    //}
                    //_buff[i] = (float)max;
                    ////double y = max;
                    ////// Limits?
                    ////y = Math.Min(y, _rawMax);
                    ////y = Math.Max(y, -_rawMax);
                    ////_buff[i] = (float)y;

                    //// Find the average value in the bin.
                    //double val = 0;
                    //for (int d = 0; d < smplPerPixel; d++)
                    //{
                    //    val += _rawVals[r];
                    //    r++;
                    //}
                    //_buff[i] = (float)val / smplPerPixel;

                    //// Find the median value in the bin.
                    //for (int d = 0; d < smplPerPixel; d++)
                    //{
                    //    r++;
                    //}
                    //_buff[i] = _rawVals[r - smplPerPixel / 2];
                }
            }
        }


        /// <summary>
        /// Update drawing area.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Rescale();
            Invalidate();
        }

        /// <summary>
        /// Simple utility.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fn"></param>
        void Dump(float[] data, string fn)
        {
            if (data != null)
            {
                List<string> ss = new List<string>();
                for (int i = 0; i < data.Length; i++)
                {
                    ss.Add($"{i + 1}, {data[i]}");
                }
                File.WriteAllLines(fn, ss);
            }
        }
        #endregion
    }
}
