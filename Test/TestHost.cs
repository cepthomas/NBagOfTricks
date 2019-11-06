using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    public partial class TestHost : Form
    {
        public TestHost()
        {
            InitializeComponent();
        }

        public void RunTests()
        {
            // Use pnut for automated lib tests.
            TestRunner runner = new TestRunner(OutputFormat.Readable);
            //var cases = new[] { "PNUT", "SM", "CMD" };
            var cases = new[] { "CMD" };
            runner.RunSuites(cases);
        }

        private void TestHost_Load(object sender, EventArgs e)
        {
            txtInfo.Colors.Add("note:7", Color.Purple);
            txtInfo.Colors.Add("vel:10", Color.Green);
            txtInfo.BackColor = Color.Cornsilk;

            vkbd.ShowNoteNames = true;
        }

        private void vkbd_KeyboardEvent(object sender, UI.VirtualKeyboard.KeyboardEventArgs e)
        {
            string s = $"note:{e.NoteId} vel:{e.Velocity}";
            txtInfo.AddLine(s);
        }

        private void btnUT_Click(object sender, EventArgs e)
        {
            RunTests();
        }
    }
}
