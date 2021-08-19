using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using NBagOfTricks;
using NBagOfTricks.UI;

namespace NBagOfTricks.Test
{
    public partial class TestHost : Form
    {
        public TestHost()
        {
            InitializeComponent();
        }

        private void TestHost_Load(object sender, EventArgs e)
        {
            ///// Misc controls.
            txtInfo.Colors.Add("note:7", Color.Purple);
            txtInfo.Colors.Add("vel:10", Color.Green);
            txtInfo.BackColor = Color.Cornsilk;

            vkbd.ShowNoteNames = true;

            pot1.ValueChanged += Pot1_ValueChanged;
            pan1.ValueChanged += Pan1_ValueChanged;
            slider1.ValueChanged += Slider1_ValueChanged;
            slider2.ValueChanged += Slider2_ValueChanged;

            ///// Filter tree.
            string root = $@"C:\Dev\repos\NBagOfTricks\Test";
            ftree.RootDirs = new List<string>() { root };
            ftree.FilterExts = ".txt;.md;.xml;.cs".SplitByToken(";");
            ftree.AllTags = new Dictionary<string, bool>() { { "abc", true }, { "123", false }, { "xyz", true } };
            ftree.DoubleClickSelect = false;

            // Good files
            ftree.TaggedPaths[$@"{root}\Test_CMD.cs"] = "";
            ftree.TaggedPaths[$@"{root}\Test_PNUT.cs"] = "abc";
            ftree.TaggedPaths[$@"{root}\bin\Debug\testout.txt"] = "123 xyz";

            // Bad paths.
            //ftree.TaggedPaths.Add(($@"{root}\bad_file.txt", "xyz"));
            //ftree.TaggedPaths.Add(($@"{root}\bin\bad_path", ""));

            // Bad tags.
            ftree.TaggedPaths[$@"{root}\bin\Debug\NBagOfTricks.xml"] = "333333 abc";

            ftree.Init();

            ///// Wave viewer.
            // Simple sin.
            float[] data1 = new float[150];
            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = (float)Math.Sin(MathUtils.DegreesToRadians(i));
            }
            waveViewer1.Mode = WaveViewer.DrawMode.Raw;
            waveViewer1.DrawColor = Color.Green;
            waveViewer1.Init(data1, 1.0f);

            // Real data.
            string[] sdata = File.ReadAllLines(@".\Files\wav.txt");
            float[] data2 = new float[sdata.Length];
            for (int i = 0; i < sdata.Length; i++)
            {
                data2[i] = float.Parse(sdata[i]);
            }
            waveViewer2.Mode = WaveViewer.DrawMode.Envelope;
            waveViewer2.DrawColor = Color.Green;
            waveViewer2.Init(data2, 1.0f);

            ///// Click grid.
            clickGrid1.AddStateType(10, Color.Blue, Color.AliceBlue);
            clickGrid1.AddStateType(20, Color.AliceBlue, Color.Blue);
            clickGrid1.AddStateType(30, Color.Red, Color.Salmon);

            string[] names = { "dignissim", "cras", "tincidunt", "lobortis", "feugiat", "vivamus", "at", "augue", "eget" };

            for (int i = 0; i < names.Count(); i++)
            {
                clickGrid1.AddIndicator(names[i], 10 + i);
            }

            clickGrid1.IndicatorEvent += ClickGrid_IndicatorEvent;
            clickGrid1.Show(4, 60, 20);

            ///// Time bar.
            timeBar.SnapMsec = 10;
            timeBar.Length = new TimeSpan(0, 0, 1, 23, 456);
            timeBar.Start = new TimeSpan(0, 0, 0, 10, 333);
            timeBar.End = new TimeSpan(0, 0, 0, 44, 777);
            timeBar.CurrentTimeChanged += TimeBar_CurrentTimeChanged1;
            timeBar.ProgressColor = Color.CornflowerBlue;
            timeBar.BackColor = Color.Salmon;

            ///// Bar bar.
            barBar.BeatsPerBar = 4;
            barBar.SubdivsPerBeat = 16;
            barBar.Snap = BarBar.SnapType.Bar;
            barBar.Length = new BarSpan(16, 0, 0);
            barBar.Start = new BarSpan(2, 1, 11);
            barBar.End = new BarSpan(11, 3, 6);
            barBar.CurrentTimeChanged += BarBar1_CurrentTimeChanged;
            barBar.ProgressColor = Color.MediumPurple;
            barBar.BackColor = Color.LawnGreen;
            //barBar.Test();

            // Go-go-go.
            timer1.Enabled = true;
        }

        void Timer1_Tick(object sender, EventArgs e)
        {
            if (chkRunBars.Checked)
            {
                // Update time bar.
                timeBar.IncrementCurrent(timer1.Interval + 3); // not-real time for testing
                if (timeBar.Current >= timeBar.End) // done/reset
                {
                    timeBar.Current = timeBar.Start;
                }

                // Update bar bar.
                barBar.IncrementCurrent(1);
                if (barBar.Current >= barBar.End) // done/reset
                {
                    barBar.Current = barBar.Start;
                }
            }
        }

        private void TimeBar_CurrentTimeChanged1(object sender, EventArgs e)
        {
        }

        private void BarBar1_CurrentTimeChanged(object sender, EventArgs e)
        {
        }

        private void ClickGrid_IndicatorEvent(object sender, IndicatorEventArgs e)
        {
            clickGrid1.SetIndicator(e.Id, (e.State + 10) % 40);
        }

        private void TestHost_Shown(object sender, EventArgs e)
        {

        }

        private void TestHost_FormClosing(object sender, FormClosingEventArgs e)
        {
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

        private void ChkCpu_CheckedChanged(object sender, EventArgs e)
        {
            cpuMeter1.Enable = chkCpu.Checked;
        }

        private void TimeBar_CurrentTimeChanged(object sender, EventArgs e)
        {
            //txtInfo.AddLine($"Current time:{timeBar.CurrentTime}");
        }

        private void BtnMarkdown_Click(object sender, EventArgs e)
        {
            // Main help file.
            var mdText = new List<string>();
            mdText.AddRange(File.ReadAllLines(@"..\..\..\README.md"));
            var htmlText = Tools.MarkdownToHtml(mdText, "lightgreen", "helvetica", true); // arial, helvetica, sans-serif
        }
    }
}
