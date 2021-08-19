using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NBagOfTricks;


namespace NBagOfTricks.UI
{
    public partial class WaveViewer : UserControl
    {
        public enum DrawMode { Raw, Envelope }

        #region Fields
        /// <summary>From client.</summary>
        float[] _rawVals = null;

        /// <summary>Maximum value of _rawVals (+-).</summary>
        float _rawMax = 1.0f;

        /// <summary>Storage for display.</summary>
        float[] _buff = null;

        /// <summary>For drawing.</summary>
        readonly Pen _penDraw = new Pen(Color.Black, 1);

        /// <summary>For drawing text.</summary>
        readonly Font _textFont = new Font("Cascadia", 12, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>For drawing text.</summary>
        readonly StringFormat _format = new StringFormat() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };

        /// <summary>Ratio of data point to visual point.</summary>
        int _smplPerPixel;

        int _marker1 = -1;
        int _marker2 = -1;
        #endregion

        #region Properties
        /// <summary>For styling.</summary>
        public Color DrawColor { get { return _penDraw.Color; } set { _penDraw.Color = value; } }

        /// <summary>How to draw.</summary>
        public DrawMode Mode { get; set; } = DrawMode.Envelope;

        /// <summary>Marker 1.</summary>
        public int Marker1 { get { return _marker1; } set { _marker1 = value < 0 ? -1 : MathUtils.Constrain(value, 0, _rawVals.Length); } }

        /// <summary>Marker 2.</summary>
        public int Marker2 { get { return _marker2; } set { _marker2 = value < 0 ? -1 : MathUtils.Constrain(value, 0, _rawVals.Length); } }
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
                _penDraw.Dispose();
                _textFont.Dispose();
                _format.Dispose();
           }
           base.Dispose(disposing);
        }
        #endregion

        #region Public functions
        /// <summary>
        /// Populate with data.
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="max"></param>
        public void Init(float[] vals, float max)
        {
            //Dump(vals, "raw.csv");
            _rawVals = vals;
            _rawMax = max;

            Rescale();
            //Dump(_buff, "buff.csv");
            Invalidate();
        }

        /// <summary>
        /// Hard reset.
        /// </summary>
        public void Reset()
        {
            _rawVals = null;
            _buff = null;
            _rawMax = 0;
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

            if (_buff == null)
            {
                pe.Graphics.DrawString("No data", _textFont, Brushes.Gray, ClientRectangle, _format);
            }
            else
            {
                for (int i = 0; i < _buff.Length; i++)
                {
                    double val = _buff[i];

                    switch(Mode)
                    {
                        case DrawMode.Envelope:
                            float y1 = (float)MathUtils.Map(val, -_rawMax, _rawMax, Height, 0);
                            //float y2 = Height / 2; // Line from val to 0
                            float y2 = (float)MathUtils.Map(val, -_rawMax, _rawMax, 0, Height); // Line from +val to -val
                            pe.Graphics.DrawLine(_penDraw, i, y1, i, y2);
                            break;

                        case DrawMode.Raw:
                            // Simple dot
                            float y = (float)MathUtils.Map(val, -_rawMax, _rawMax, Height, 0);
                            pe.Graphics.DrawRectangle(_penDraw, i, y, 1, 1);
                            break;
                    }
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
                int fitWidth = Width;
                _smplPerPixel = _rawVals.Length / fitWidth;

                if(_smplPerPixel > 0)
                {
                    _buff = new float[fitWidth];

                    int r = 0; // index into raw
                    for (int i = 0; i < fitWidth; i++)
                    {
                        var rms = MathUtils.RMS(_rawVals.Subset(r, _smplPerPixel));
                        _buff[i] = rms;
                        r += _smplPerPixel;
                    }
                }
                else
                {
                    _buff = new float[fitWidth];
                    for (int i = 0; i < _rawVals.Length; i++)
                    {
                        _buff[i] = _rawVals[i];
                    }
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
