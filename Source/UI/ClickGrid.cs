using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NBagOfTricks.Utils;


namespace NBagOfTricks.UI
{
    /// <summary>
    /// ClickGrid control.
    /// </summary>
    public partial class ClickGrid : UserControl
    {
        #region Fields

        List<int> _states = new List<int>();

        Dictionary<int, IndicatorStateType> _stateTypes = new Dictionary<int, IndicatorStateType>();
        
        int _numTargets = 6;

        int _cols = 2;
        int _rows = 2;

        int _indWidth = 100;

        int _indHeight = 25;

        SolidBrush _defaultForeBrush = new SolidBrush(Color.Black);

        SolidBrush _defaultBackBrush = new SolidBrush(Color.White);

        /// <summary>The pen.</summary>
        Pen _pen = new Pen(Color.Black);
        #endregion

        #region Properties
        /// <summary>Optional text, in order of indicator.</summary>
        public List<string> IndicatorText { get; set; } = new List<string>();
        #endregion

        #region Events
        /// <summary>ClickGrid value changed event.</summary>
        public event EventHandler<IndicatorEventArgs> IndicatorEvent;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Creates a new default control.
        /// </summary>
        public ClickGrid()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Load += ClickGrid_Load;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ClickGrid_Load(object sender, EventArgs e)
        {
            // Init the statuses.
            _stateTypes = new Dictionary<int, IndicatorStateType>();
        }

        /// <summary>
        /// Normal construction.
        /// </summary>
        public void Init(int numTargets, int cols, int indWidth, int indHeight)
        {
            _numTargets = numTargets;
            _cols = cols;
            _rows = _numTargets / _cols + 1;
            _indWidth = indWidth;
            _indHeight = indHeight;
            _states = new List<int>(new int[_numTargets]);
            Invalidate();
        }
        #endregion

        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="which"></param>
        /// <param name="state"></param>
        public void SetIndicator(int which, int state)
        {
            if (which >= 0 && which < _states.Count && _stateTypes.ContainsKey(state))
            {
                _states[which] = state;
                Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        public void AddStateType(int state, Color foreColor, Color backColor)
        {
            _stateTypes[state] = new IndicatorStateType()
            {
                ForeBrush = new SolidBrush(foreColor),
                BackBrush = new SolidBrush(backColor)
            };
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
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    SolidBrush fb = _defaultForeBrush;
                    SolidBrush bb = _defaultBackBrush;

                    int ind = row * _cols + col;

                    if(ind < _states.Count)
                    {
                        int state = _states[ind];
                        if (_stateTypes.ContainsKey(state))
                        {
                            fb = _stateTypes[state].ForeBrush;
                            bb = _stateTypes[state].BackBrush;
                        }
                    }

                    int x = col * _indWidth;
                    int y = row * _indHeight;
                    Rectangle r = new Rectangle(x, y, _indWidth, _indHeight);
                    pe.Graphics.FillRectangle(bb, r);

                    // Border
                    pe.Graphics.DrawRectangle(_pen, r);

                    // Text
                    string text = ind < IndicatorText.Count ? IndicatorText[ind] : $"Indicator{ind}";
                    SizeF stext = pe.Graphics.MeasureString(text, Font);
                    pe.Graphics.DrawString(text, Font, fb, x + 5, y + (_indHeight - stext.Height) / 2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            Invalidate();
        }
        #endregion

        #region Mouse events
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    break;

                case MouseButtons.Right:
                    break;
            }

            int col = e.X / _indWidth;
            int row = e.Y / _indHeight;
            int ind = row * _cols + col;

            if (ind < _numTargets)
            {
                IndicatorEvent?.Invoke(this, new IndicatorEventArgs() { Index = ind, State = _states[ind] });
            }

            base.OnMouseClick(e);
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                _defaultForeBrush.Dispose();
                _defaultBackBrush.Dispose();
                _stateTypes.ForEach(st => { st.Value.ForeBrush.Dispose(); st.Value.BackBrush.Dispose(); });
            }
            base.Dispose(disposing);
        }
        #endregion
   }

    /// <summary>
    /// 
    /// </summary>
    public class IndicatorEventArgs : EventArgs
    {
        public int Index { get; set; }
        public int State { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    class IndicatorStateType
    {
        /// <summary>The foreground brush/pen.</summary>
        public SolidBrush ForeBrush { get; set; } = null;

        /// <summary>The background brush.</summary>
        public SolidBrush BackBrush { get; set; } = null;
    }
}
