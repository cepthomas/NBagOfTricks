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
        /// <summary>State enumeration.</summary>
        Dictionary<int, IndicatorStateType> _stateTypes = new Dictionary<int, IndicatorStateType>();

        /// <summary>All the indicators.</summary>
        List<Indicator> _indicators = new List<Indicator>();

        /// <summary>Number of columns.</summary>
        int _cols = 2;

        /// <summary>Number of rows.</summary>
        int _rows = 2;

        /// <summary>Indicator geometry.</summary>
        int _indWidth = 100;

        /// <summary>Indicator geometry.</summary>
        int _indHeight = 25;

        /// <summary>Used for unspecified states.</summary>
        SolidBrush _defaultForeBrush = new SolidBrush(Color.Black);

        /// <summary>Used for unspecified states.</summary>
        SolidBrush _defaultBackBrush = new SolidBrush(Color.White);

        /// <summary>The pen.</summary>
        Pen _pen = new Pen(Color.Black);
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
            BorderStyle = BorderStyle.FixedSingle;

            // Init the statuses.
            _stateTypes = new Dictionary<int, IndicatorStateType>();
        }
        #endregion

        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="id"></param>
        public void AddIndicator(string text, int id)
        {
            _indicators.Add(new Indicator() { Text = text, Id = id });
        }

        /// <summary>
        /// Normal construction.
        /// </summary>
        public void Show(int cols, int indWidth, int indHeight)
        {
            _cols = cols;
            _rows = _indicators.Count / _cols + 1;
            _indWidth = indWidth;
            _indHeight = indHeight;
            Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public void SetIndicator(int id, int state)
        {
            var ind = _indicators.Find(i => i.Id == id);

            if (ind != null && _stateTypes.ContainsKey(state))
            {
                ind.State = state;
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

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _indicators.Clear();
            Invalidate();
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

                    if(ind < _indicators.Count)
                    {
                        int state = _indicators[ind].State;
                        if (_stateTypes.ContainsKey(state))
                        {
                            fb = _stateTypes[state].ForeBrush;
                            bb = _stateTypes[state].BackBrush;
                        }

                        int x = col * _indWidth;
                        int y = row * _indHeight;
                        Rectangle r = new Rectangle(x, y, _indWidth, _indHeight);
                        pe.Graphics.FillRectangle(bb, r);

                        // Border
                        //pe.Graphics.DrawRectangle(_pen, r);

                        // Text
                        string text = _indicators[ind].Text;
                        SizeF stext = pe.Graphics.MeasureString(text, Font);
                        pe.Graphics.DrawString(text, Font, fb, x + 5, y + (_indHeight - stext.Height) / 2);
                    }
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

            if (ind < _indicators.Count)
            {
                IndicatorEvent?.Invoke(this, new IndicatorEventArgs()
                {
                    Id = _indicators[ind].Id,
                    State = _indicators[ind].State
                });
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
    /// Click event data.
    /// </summary>
    public class IndicatorEventArgs : EventArgs
    {
        public int Id { get; set; }
        public int State { get; set; }
    }

    /// <summary>
    /// Everything about one indicator.
    /// </summary>
    class Indicator
    {
        /// <summary>User tag.</summary>
        public int Id { get; set; } = -1;

        /// <summary>The text.</summary>
        public string Text { get; set; } = "???";

        /// <summary>User tag.</summary>
        public int State { get; set; } = 0;
    }

    /// <summary>
    /// Describes one state.
    /// </summary>
    class IndicatorStateType
    {
        /// <summary>The foreground brush/pen.</summary>
        public SolidBrush ForeBrush { get; set; } = null;

        /// <summary>The background brush.</summary>
        public SolidBrush BackBrush { get; set; } = null;
    }
}
