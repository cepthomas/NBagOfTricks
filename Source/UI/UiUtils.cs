using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using NBagOfTricks.Utils;


namespace NBagOfTricks.UI
{
    /// <summary>Custom renderer for toolstrip checkbox color.</summary>
    public class CheckBoxRenderer : ToolStripSystemRenderer
    {
        /// <summary>
        /// Color to use when check box is selected.
        /// </summary>
        public Color SelectedColor { get; set; }

        /// <summary>
        /// Override for drawing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            var btn = e.Item as ToolStripButton;

            if (!(btn is null) && btn.CheckOnClick && btn.Checked)
            {
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                e.Graphics.FillRectangle(new SolidBrush(SelectedColor), bounds);
            }
            else
            {
                base.OnRenderButtonBackground(e);
            }
        }
    }

    /// <summary>Generic property editor for lists of strings.</summary>
    public class ListEditor : UITypeEditor
    {
        private IWindowsFormsEditorService _service = null;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            List<string> ls = value as List<string>;

            if (ls != null && ls.Count > 0)
            {
                TextBox tb = new TextBox
                {
                    Multiline = true,
                    ReadOnly = false,
                    AcceptsReturn = true,
                    ScrollBars = ScrollBars.Both,
                    Height = ls.Count * 30,
                    Text = string.Join(Environment.NewLine, ls)
                };
                _service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                _service.DropDownControl(tb);
                ls = tb.Text.SplitByTokens(Environment.NewLine);
            }

            return ls;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.DropDown; }
    }

    /// <summary>Selector for monospaced fonts.</summary>
    public class MonospaceFontEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            FontDialog dlg = new FontDialog()
            {
                FixedPitchOnly = true,
                Font = value as Font
            };

            return dlg.ShowDialog() == DialogResult.OK ? dlg.Font : base.EditValue(context, provider, value);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.Modal; }
    }
}
