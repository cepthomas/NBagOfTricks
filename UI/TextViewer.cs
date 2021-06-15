using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NBagOfTricks.UI
{
    public class TextViewer : UserControl
    {
        #region Properties
        /// <summary>The colors to display when text is matched.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, Color> Colors { get; set; } = new Dictionary<string, Color>();

        /// <summary>User selectable color.</summary>
        public new Color BackColor { get { return _rtb.BackColor; } set { _rtb.BackColor = value; } }

        /// <summary>User selectable font.</summary>
        public new Font Font { get { return _rtb.Font; } set { _rtb.Font = value; } }

        /// <summary>User selection.</summary>
        public bool WordWrap { get { return _rtb.WordWrap; } set { _rtb.WordWrap = value; } }

        /// <summary>Limit the size.</summary>
        public int MaxText { get; set; } = 5000;
        #endregion

        #region Fields
        /// <summary>Contained control. Could have just subclassed but this leaves it open to add other stuff easily.</summary>
        RichTextBox _rtb = new RichTextBox();
        #endregion

        /// <summary>
        /// Constructor sets some defaults.
        /// </summary>
        public TextViewer()
        {
            Controls.Add(_rtb);
            Font = new Font("Consolas", 10);
            Load += new EventHandler(TextViewer_Load);
        }

        /// <summary>
        /// Initialize everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextViewer_Load(object sender, EventArgs e)
        {
            BorderStyle = BorderStyle.None;

            _rtb.Text = "";
            _rtb.BorderStyle = BorderStyle.None;
            _rtb.ForeColor = Color.Black;
            _rtb.Dock = DockStyle.Fill;
            _rtb.ReadOnly = true;
            _rtb.ScrollBars = RichTextBoxScrollBars.Both;
        }

        /// <summary>
        /// A message to display to the user. Adds EOL.
        /// </summary>
        /// <param name="text">The message.</param>
        /// <param name="trim">True to truncate continuous displays.</param>
        public void AddLine(string text, bool trim = true)
        {
            Add($"{text}{Environment.NewLine}", trim);
        }

        /// <summary>
        /// A message to display to the user. Doesn't add EOL.
        /// </summary>
        /// <param name="text">The message.</param>
        /// <param name="trim">True to truncate continuous displays.</param>
        public void Add(string text, bool trim = true)
        {
            if (trim && _rtb.TextLength > MaxText)
            {
                _rtb.Select(0, MaxText / 5);
                _rtb.SelectedText = "";
            }

            _rtb.SelectionBackColor = BackColor; // default

            foreach (string s in Colors.Keys)
            {
                if (text.Contains(s))
                {
                    _rtb.SelectionBackColor = Colors[s];
                    break;
                }
            }

            _rtb.AppendText(text);
            _rtb.ScrollToCaret();
        }

        /// <summary>
        /// Remove all text.
        /// </summary>
        public void Clear()
        {
            _rtb.Clear();
        }

        /// <summary>5
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rtb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
