using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        void Run_Click(object sender, EventArgs e)
        {
            // Use pnut for automated lib tests.
            TestRunner runner = new(OutputFormat.Readable);
            var cases = new[] { "TOOLS" };
            //var cases = new[] { "PNUT", "UTILS", "CMD", "MMTEX", "IPC", "TOOLS", "JSON", "SLOG" };
            runner.RunSuites(cases);
            rtbOut.AppendText(string.Join(Environment.NewLine, runner.Context.OutputLines));

        }
    }
}
