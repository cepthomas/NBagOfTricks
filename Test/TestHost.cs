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


namespace NBagOfTricks.Test
{
    public partial class TestHost : Form // TODOC resize is klunky.
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

            List<string> paths = new List<string>() { $@"{Environment.CurrentDirectory}\..\..\" };
            List<string> exts = ".txt;.md;.xml;.cs".SplitByToken(";");
            tvex.AllTags = new HashSet<string>() { "abc", "123", "xyz" };
            tvex.DoubleClickSelect = false;
            tvex.Init(paths, exts);

            //C:\Dev\repos\NBagOfTricks\Test
            //| Program.cs
            //| TestHost.cs
            //| TestHost.Designer.cs
            //| Test_CMD - Copy.cs
            //| Test_CMD.cs
            //| Test_PNUT.cs
            //| Test_SM.cs
            //+ ---bin
            //|   \---Debug
            //| NBagOfTricks.xml
            //| testout.txt
            //+ ---obj
            //|   \---Debug
            //|       |   .NETFramework,Version = v4.7.1.AssemblyAttributes.cs
            //|       | TemporaryGeneratedFile_036C0B5B - 1481 - 4323 - 8D20 - 8F5ADCB23D92.cs
            //|       | TemporaryGeneratedFile_5937a670 - 0e60 - 4077 - 877b - f7221da3dda1.cs
            //|       | TemporaryGeneratedFile_E7A71F73 - 0F8D - 4B9B - B56E - 8E70B10BC5D3.cs
            //|       | Test.csproj.FileListAbsolute.txt
            //\---Properties
            //        AssemblyInfo.cs
        }

        void TreeViewEx_FileSelectedEvent(object sender, string fn)
        {
            txtInfo.AddLine($"Selected file: {fn}");
        }

        private void Pot1_ValueChanged(object sender, EventArgs e)
        {
            // 0 1
            meter2.AddValue(pot1.Value);
        }

        private void Slider1_ValueChanged(object sender, EventArgs e)
        {
            // 0 1
            meter1.AddValue(slider1.Value * 100.0);
        }

        private void Slider2_ValueChanged(object sender, EventArgs e)
        {
            // 0 10
            meter1.AddValue(slider2.Value * 10.0);
        }

        private void Pan1_ValueChanged(object sender, EventArgs e)
        {
            // -1 +1
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
    }
}
