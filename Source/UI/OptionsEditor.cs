using NBagOfTricks.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NBagOfTricks.UI
{
    public partial class OptionsEditor : Form
    {
        #region Fields
        /// <summary>Working values so we don't destroy originals.</summary>
        Dictionary<string, bool> _values;
        #endregion

        #region Properties
        /// <summary>The values to edit. Key is text, value is bool enable. Clone in case user cancels.</summary>
        public Dictionary<string, bool> Values
        { 
            get { return _values; }
            set { _values = new Dictionary<string, bool>(value); _values.ForEach(kv => lbValues.Items.Add(kv.Key, kv.Value)); }
        }

        /// <summary>Custom label.</summary>
        public string Title { get; set; } = "Options Editor";

        /// <summary>If true, user can add and delete values, otherwise just select.</summary>
        public bool AllowEdit { get; set; } = false;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialization. If editing not allowed, adjust the ui.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OptionsEditor_Load(object sender, EventArgs e)
        {
            if(!AllowEdit)
            {
                btnAdd.Visible = false;
                txtAdd.Visible = false;
                lbValues.Height = lbValues.Top + btnAdd.Bottom;
            }
        }

        /// <summary>
        /// User is adding a new value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Add_Click(object sender, EventArgs e)
        {
            // If textbox is not empty, add to collection.
            string s = txtAdd.Text.Trim().Replace(" ", "_");
            if (s.Length > 0)
            {
                lbValues.Items.Add(s, true);
            }
        }

        /// <summary>
        /// User might be removing a value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Values_KeyDown(object sender, KeyEventArgs e)
        {
            if(AllowEdit && e.KeyCode == Keys.Delete)
            {
                lbValues.Items.RemoveAt(lbValues.SelectedIndex);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Collect list contents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OptionsEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _values.Clear();

            for(int i = 0; i < lbValues.Items.Count; i++)
            {
                _values[lbValues.Items[i].ToString()] = lbValues.GetItemChecked(i);
            }
        }
    }
}
