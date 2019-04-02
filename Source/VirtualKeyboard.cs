using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;

namespace NBagOfTricks.UI
{
    /// <summary>
    /// Virtual keyboard control borrowed from Leslie Sanford with extras.
    /// </summary>
    public partial class VirtualKeyboard : UserControl
    {
        #region Events
        /// <summary>Device has received something.</summary>
        public class KeyboardEventArgs : EventArgs
        {
            /// <summary>Channel number.</summary>
            public int ChannelNumber { get; set; } = 1; // TODO editable?

            /// <summary>Midi note id.</summary>
            public int NoteId { get; set; } = 0;

            /// <summary>Midi velocity.</summary>
            public double Velocity { get; set; } = 0;
        }
        public event EventHandler<KeyboardEventArgs> KeyboardEvent;
        #endregion

        #region Constants
        const int LOW_NOTE = 21;
        const int HIGH_NOTE = 109;
        const int MIDDLE_C = 60;
        #endregion

        #region Fields
        /// <summary>All the created piano keys.</summary>
        List<VirtualKey> _keys = new List<VirtualKey>();

        /// <summary>Map from Keys value to the index in _keys.</summary>
        Dictionary<Keys, int> _keyMap = new Dictionary<Keys, int>();

        /// <summary>Known bug?</summary>
        bool _keyDown = false;
        #endregion

        #region Properties
        /// <inheritdoc />
        public string DeviceName { get; private set; } = "";
        #endregion

        #region Lifecycle
        /// <summary>
        /// Constructor.
        /// </summary>
        public VirtualKeyboard()
        {
            CreateKeys();
            // Intercept all keyboard events.
            //KeyPreview = true;

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(773, 173);
            Name = "VirtualKeyboard";
            Text = "Virtual Keyboard";
            Load += Keyboard_Load;
            KeyDown += Keyboard_KeyDown;
            KeyUp += Keyboard_KeyUp;
            Resize += Keyboard_Resize;
        }

        /// <summary>
        /// Initialize everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Keyboard_Load(object sender, EventArgs e)
        {
            //// Get the bitmap.
            //Bitmap bm = new Bitmap(Properties.Resources.glyphicons_327_piano);

            //// Convert to an icon and use for the form's icon.
            //Icon = Icon.FromHandle(bm.GetHicon());
        }

        /// <inheritdoc />
        public bool Init(string _ = "")
        {
            bool inited = true;

            DrawKeys();

            CreateKeyMap();

            Show();

            return inited;
        }
        #endregion

        #region Designer boilerplate
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        //#region Interface implementation
        ///// <inheritdoc />
        //public void Start()
        //{
        //    // Ignore.
        //}

        ///// <inheritdoc />
        //public void Stop()
        //{
        //    // Ignore.
        //}

        ///// <inheritdoc />
        //public void Housekeep()
        //{
        //    // Ignore.
        //}
        //#endregion

        #region Private functions
        /// <summary>
        /// Is it a white key?
        /// </summary>
        /// <param name="notenum">Which note</param>
        /// <returns>True/false</returns>
        public bool IsNatural(int notenum)
        {
            int[] naturals = { 0, 2, 4, 5, 7, 9, 11 };
            return naturals.Contains(notenum % 12);
        }

        /// <summary>
        /// Create the midi note/keyboard mapping.
        /// </summary>
        void CreateKeyMap()
        {
            _keyMap.Clear();

            int indexOfMiddleC = _keys.IndexOf(_keys.Where(k => k.NoteId == MIDDLE_C).First());

            string[] keyDefs =
            {
                    "Z  -12  ;  C-3",
                    "S  -11  ;  C#",
                    "X  -10  ;  D",
                    "D  -9   ;  D#",
                    "C  -8   ;  E",
                    "V  -7   ;  F",
                    "G  -6   ;  F#",
                    "B  -5   ;  G",
                    "H  -4   ;  G#",
                    "N  -3   ;  A",
                    "J  -2   ;  A#",
                    "M  -1   ;  B",
                    ",   0   ;  C-4",
                    "L  +1   ;  C#-4",
                    ".  +2   ;  D",
                    ";  +3   ;  D#",
                    "/  +4   ;  E-4",
                    "Q   0   ;  C-4",
                    "2  +1   ;  C#",
                    "W  +2   ;  D",
                    "3  +3   ;  D#",
                    "E  +4   ;  E",
                    "R  +5   ;  F",
                    "5  +6   ;  F#",
                    "T  +7   ;  G",
                    "6  +8   ;  G#",
                    "Y  +9   ;  A",
                    "7  +10  ;  A#",
                    "U  +11  ;  B",
                    "I  +12  ;  C-5",
                    "9  +13  ;  C#-5",
                    "O  +14  ;  D",
                    "0  +15  ;  D#",
                    "P  +16  ;  E-5"
                };

            foreach (string l in keyDefs)
            {
                List<string> parts = l.SplitByToken(" ");

                if (parts.Count >= 2 && parts[0] != ";")
                {
                    string key = parts[0];
                    char ch = key[0];
                    int offset = int.Parse(parts[1]);
                    int note = indexOfMiddleC + offset;

                    switch (key)
                    {
                        case ",": _keyMap.Add(Keys.Oemcomma, note); break;
                        case "=": _keyMap.Add(Keys.Oemplus, note); break;
                        case "-": _keyMap.Add(Keys.OemMinus, note); break;
                        case "/": _keyMap.Add(Keys.OemQuestion, note); break;
                        case ".": _keyMap.Add(Keys.OemPeriod, note); break;
                        case "\'": _keyMap.Add(Keys.OemQuotes, note); break;
                        case "\\": _keyMap.Add(Keys.OemPipe, note); break;
                        case "]": _keyMap.Add(Keys.OemCloseBrackets, note); break;
                        case "[": _keyMap.Add(Keys.OemOpenBrackets, note); break;
                        case "`": _keyMap.Add(Keys.Oemtilde, note); break;
                        case ";": _keyMap.Add(Keys.OemSemicolon, note); break;

                        default:
                            if ((ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
                            {
                                _keyMap.Add((Keys)ch, note);
                            }
                            break;
                    }
                }
            }
        }
        #endregion

        #region User input handlers
        /// <summary>
        /// Event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Keyboard_Resize(object sender, EventArgs e)
        {
            DrawKeys();
        }

        /// <summary>
        /// Use alpha keyboard to drive piano.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Keyboard_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_keyDown)
            {
                if (_keyMap.ContainsKey(e.KeyCode))
                {
                    VirtualKey pk = _keys[_keyMap[e.KeyCode]];
                    if (!pk.IsPressed)
                    {
                        pk.PressVKey();
                        e.Handled = true;
                    }
                }

                _keyDown = true;
            }
        }

        /// <summary>
        /// Use alpha keyboard to drive piano.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Keyboard_KeyUp(object sender, KeyEventArgs e)
        {
            _keyDown = false;

            if (_keyMap.ContainsKey(e.KeyCode))
            {
                VirtualKey pk = _keys[_keyMap[e.KeyCode]];
                pk.ReleaseVKey();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Pass along an event from a virtual key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Keyboard_VKeyEvent(object sender, VirtualKey.VKeyEventArgs e)
        {
            if (KeyboardEvent != null)
            {
                KeyboardEvent.Invoke(this, new KeyboardEventArgs()
                {
                    ChannelNumber = 1,
                    NoteId = e.NoteId,
                    Velocity = 0.8
                });
            }
        }
        #endregion

        #region Drawing
        /// <summary>
        /// Create the key controls.
        /// </summary>
        void CreateKeys()
        {
            _keys.Clear();

            for (int i = 0; i < HIGH_NOTE - LOW_NOTE; i++)
            {
                int noteId = i + LOW_NOTE;
                VirtualKey pk;
                if (IsNatural(noteId))
                {
                    pk = new VirtualKey(this, true, noteId);
                }
                else
                {
                    pk = new VirtualKey(this, false, noteId);
                    pk.BringToFront();
                }

                pk.VKeyEvent += Keyboard_VKeyEvent;
                _keys.Add(pk);
                Controls.Add(pk);
            }
        }

        /// <summary>
        /// Re/draw the keys.
        /// </summary>
        void DrawKeys()
        {
            int whiteKeyWidth = Width / _keys.Count(k => k.IsNatural);
            int blackKeyWidth = (int)(whiteKeyWidth * 0.6);
            int blackKeyHeight = (int)(Height * 0.5);
            int offset = whiteKeyWidth - blackKeyWidth / 2;

            int w = 0;

            for (int i = 0; i < _keys.Count; i++)
            {
                VirtualKey pk = _keys[i];

                if (pk.IsNatural)
                {
                    pk.Height = Height;
                    pk.Width = whiteKeyWidth;
                    pk.Location = new Point(w * whiteKeyWidth, 0);
                    w++;
                }
                else
                {
                    pk.Height = blackKeyHeight;
                    pk.Width = blackKeyWidth;
                    pk.Location = new Point(offset + (w - 1) * whiteKeyWidth);
                    pk.BringToFront();
                }
            }
        }
        #endregion
    }

    /// <summary></summary>
    public class VirtualKey : Control
    {
        #region Fields
        /// <summary>Hook to owner.</summary>
        VirtualKeyboard _owner;
        #endregion

        #region Properties
        /// <summary>Key status.</summary>
        public bool IsPressed { get; private set; } = false;

        /// <summary>Key status.</summary>
        public bool IsNatural { get; private set; } = false;

        /// <summary>Associated midi note.</summary>
        public int NoteId { get; private set; } = 0;
        #endregion

        #region Events
        /// <summary>Notify handlers of key change.</summary>
        public event EventHandler<VKeyEventArgs> VKeyEvent;
        public class VKeyEventArgs : EventArgs
        {
            public int NoteId { get; set; }
            public bool Down { get; set; }
        }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="isNatural"></param>
        /// <param name="noteId"></param>
        public VirtualKey(VirtualKeyboard owner, bool isNatural, int noteId)
        {
            _owner = owner;
            TabStop = false;
            IsNatural = isNatural;
            NoteId = noteId;
        }
        #endregion

        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        public void PressVKey()
        {
            IsPressed = true;
            Invalidate();
            VKeyEvent?.Invoke(this, new VKeyEventArgs() { NoteId = NoteId, Down = true });
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReleaseVKey()
        {
            IsPressed = false;
            Invalidate();
            VKeyEvent?.Invoke(this, new VKeyEventArgs() { NoteId = NoteId, Down = false });
        }
        #endregion

        #region Mouse handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                PressVKey();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsPressed)
            {
                ReleaseVKey();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            PressVKey();

            if (!_owner.Focused)
            {
                _owner.Focus();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            ReleaseVKey();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.X < 0 || e.X > Width || e.Y < 0 || e.Y > Height)
            {
                Capture = false;
            }
        }
        #endregion

        #region Draw the control
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsPressed)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.SkyBlue), 0, 0, Size.Width, Size.Height);
            }
            else
            {
                e.Graphics.FillRectangle(IsNatural ? new SolidBrush(Color.White) : new SolidBrush(Color.Black), 0, 0, Size.Width, Size.Height);
            }

            e.Graphics.DrawRectangle(Pens.Black, 0, 0, Size.Width - 1, Size.Height - 1);
        }
        #endregion
    }
}
