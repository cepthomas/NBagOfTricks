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
        /// <summary>
        /// The colors to display when text is matched.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, Color> Colors { get; set; } = new Dictionary<string, Color>();

        /// <summary>
        /// User selectable color.
        /// </summary>
        public override Color BackColor { get; set; } = Color.AliceBlue;

        /// <summary>
        /// User selectable font.
        /// </summary>
        public override Font Font { get; set; } = new Font("Consolas", 9);

        /// <summary>
        /// User selection.
        /// </summary>
        public bool WordWrap { set { _rtb.WordWrap = value; } }

        /// <summary>
        /// Limit the size.
        /// </summary>
        public int MaxText { get; set; } = 5000;
        #endregion

        #region Fields
        /// <summary>
        /// Contained control. Could have just subclassed but this leaves it open to add other stuff easily.
        /// </summary>
        RichTextBox _rtb;
        #endregion

        /// <summary>
        /// Constructor sets some defaults.
        /// </summary>
        public TextViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextViewer_Load(object sender, EventArgs e)
        {
            _rtb.Font = Font;
            _rtb.BackColor = BackColor;
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
            Add(text + Environment.NewLine, trim);
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

        #region Designer boilerplate
        /// <summary>
        /// Required designer variable.
        /// </summary>
        System.ComponentModel.IContainer components = new System.ComponentModel.Container();

        /// <summary>5
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

        private void InitializeComponent()
        {
            _rtb = new RichTextBox();
            Controls.Add(_rtb);

            this.SuspendLayout();
            this.Name = "TextViewer";
            this.Load += new System.EventHandler(this.TextViewer_Load);
            this.ResumeLayout(false);
        }
        #endregion
    }
}
