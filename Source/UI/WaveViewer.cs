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

        /// <summary>A number.</summary>
        const int BORDER_WIDTH = 1;
        #endregion

        #region Properties
        /// <summary>For styling.</summary>
        public Color ControlColor { get; set; } = Color.Orange;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public WaveViewer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
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

        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="max"></param>
        public void Init(float[] vals, float max)
        {
            _rawVals = vals;
            _rawMax = max;

            Rescale();
            Invalidate();
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
                int fitWidth = Width - 2 * BORDER_WIDTH;
                _buff = new float[fitWidth];
                int smplPerPixel = _rawVals.Length / fitWidth;

                int r = 0; // index into raw
                for (int i = 0; i < fitWidth; i++)
                {
                    double max = 0;
                    for (int d = 0; d < smplPerPixel; d++)
                    {
                        max = Math.Max(max, Math.Abs(_rawVals[r]));
                        r++;
                    }
                    double y = max;

                    // Limits?
                    y = Math.Min(y, _rawMax);
                    y = Math.Max(y, -_rawMax);
                    _buff[i] = (float)y;
                }
            }
        }

        /// <summary>
        /// Paints the waveform.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Setup.
            pe.Graphics.Clear(BackColor);
            Brush brush = new SolidBrush(ControlColor);
            Pen pen = new Pen(ControlColor);

            // Draw border.
            Pen penBorder = new Pen(Color.Black, BORDER_WIDTH);
            pe.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            if (_buff == null)
            {
                Rectangle r = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height / 2);
                pe.Graphics.DrawString("No data - please init()", Font, Brushes.Red, r);
            }
            else
            {
                for (int i = 0; i < _buff.Length; i++)
                {
                    double val = _buff[i];

                    // Draw data point.
                    double x = i + BORDER_WIDTH;
                    double y1 = MathUtils.Map(val, -_rawMax, _rawMax, Height - 2 * BORDER_WIDTH, BORDER_WIDTH);
                    double y2 = MathUtils.Map(val, -_rawMax, _rawMax, BORDER_WIDTH, Height - 2 * BORDER_WIDTH);
                    pe.Graphics.DrawLine(pen, (float)x, (float)y1, (float)x, (float)y2);
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
        #endregion
    }
}
