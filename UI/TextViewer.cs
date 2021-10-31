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
    public class TextViewer : RichTextBox
    {
        #region Properties
        /// <summary>The colors to display when text is matched.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, Color> Colors { get; set; } = new Dictionary<string, Color>();

        /// <summary>Limit the size.</summary>
        public int MaxText { get; set; } = 50000;
        #endregion

        /// <summary>
        /// Constructor sets some defaults.
        /// </summary>
        public TextViewer()
        {
            Font = new Font("Consolas", 10);
        }

        /// <summary>
        /// Initialize everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextViewer_Load(object sender, EventArgs e)
        {
            Text = "";
            BorderStyle = BorderStyle.None;
            ForeColor = Color.Black;
            Dock = DockStyle.Fill;
            ReadOnly = true;
            ScrollBars = RichTextBoxScrollBars.Both;
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
            if (trim && TextLength > MaxText)
            {
                Select(0, MaxText / 5);
                SelectedText = "";
            }

            SelectionBackColor = BackColor; // default

            foreach (string s in Colors.Keys)
            {
                if (text.Contains(s))
                {
                    SelectionBackColor = Colors[s];
                    break;
                }
            }

            AppendText(text);
            ScrollToCaret();
        }
    }
}
