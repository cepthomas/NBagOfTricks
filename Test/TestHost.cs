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
using NBagOfTricks.Utils;
using NBagOfTricks.UI;
using System.IO;

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
            var cases = new[] { "PNUT", "SM", "CMD" };
            runner.RunSuites(cases);
        }

        private void TestHost_Load(object sender, EventArgs e)
        {
            txtInfo.Colors.Add("note:7", Color.Purple);
            txtInfo.Colors.Add("vel:10", Color.Green);
            txtInfo.BackColor = Color.Cornsilk;

            vkbd.ShowNoteNames = true;

            pot1.ValueChanged += Pot1_ValueChanged;
            pan1.ValueChanged += Pan1_ValueChanged;
            slider1.ValueChanged += Slider1_ValueChanged;
            slider2.ValueChanged += Slider2_ValueChanged;

            string root = $@"C:\Dev\repos\NBagOfTricks\Test";
            ftree.RootPaths = new List<string>() { root };
            ftree.FilterExts = ".txt;.md;.xml;.cs".SplitByToken(";");
            ftree.AllTags = new List<string>() { "abc", "123", "xyz" };
            ftree.DoubleClickSelect = false;

            // Good files
            ftree.TaggedPaths.Add(($@"{root}\Test_CMD.cs", ""));
            ftree.TaggedPaths.Add(($@"{root}\Test_PNUT.cs", "abc"));
            ftree.TaggedPaths.Add(($@"{root}\bin\Debug\testout.txt", "123 xyz"));

            // Bad paths.
            //ftree.TaggedPaths.Add(($@"{root}\bad_file.txt", "xyz"));
            //ftree.TaggedPaths.Add(($@"{root}\bin\bad_path", ""));

            // Bad tags.
            ftree.TaggedPaths.Add(($@"{root}\bin\Debug\NBagOfTricks.xml", "333333 abc"));

            ftree.Init();

            timeControl.Length = new TimeSpan(0, 12, 34);
            timeControl.CurrentTime = new TimeSpan(0);

            timer1.Enabled = true;
        }

        private void TestHost_Shown(object sender, EventArgs e)
        {

        }

        private void TestHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            ftree.FlushChanges();

            // Inspect.
            var at = ftree.AllTags;
            var tp = ftree.TaggedPaths;
        }

        void FilTree_FileSelectedEvent(object sender, string fn)
        {
            txtInfo.AddLine($"Selected file: {fn}");
        }

        private void Pot1_ValueChanged(object sender, EventArgs e)
        {
            // 0 -> 1
            meter2.AddValue(pot1.Value);
        }

        private void Slider1_ValueChanged(object sender, EventArgs e)
        {
            // 0 -> 1
            meter1.AddValue(slider1.Value * 100.0);
        }

        private void Slider2_ValueChanged(object sender, EventArgs e)
        {
            // 0 -> 10
            meter1.AddValue(slider2.Value * 10.0);
        }

        private void Pan1_ValueChanged(object sender, EventArgs e)
        {
            // -1 -> +1
            meter1.AddValue(pan1.Value * 50.0 + 50.0);
        }

        private void Vkbd_KeyboardEvent(object sender, VirtualKeyboard.KeyboardEventArgs e)
        {
            string s = $"note:{e.NoteId} vel:{e.Velocity}";
            txtInfo.AddLine(s);

            meter3.AddValue(e.NoteId / 8.0 - 10.0);
        }

        private void UT_Click(object sender, EventArgs e)
        {
            RunTests();
        }

        private void ChkCpu_CheckedChanged(object sender, EventArgs e)
        {
            cpuMeter1.Enable = chkCpu.Checked;
        }

        private void TimeControl_CurrentTimeChanged(object sender, EventArgs e)
        {
            //txtInfo.AddLine($"Current time:{timeControl.CurrentTime}");
        }

        void Timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = timeControl.CurrentTime + new TimeSpan(0, 0, 0, 0, timer1.Interval);
            timeControl.CurrentTime = ts < timeControl.Length ? ts : timeControl.Length;
        }
    }
}
