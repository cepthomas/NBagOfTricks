using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json.Serialization;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class TOOLS_SCRIPT : TestSuite
    {
        public override void RunSuite()
        {
            StopOnFail(true);

            Info("Test RunScript().");

            var stdout = new List<string>();
            var stderr = new List<string>();

            void _print(string text) { stdout.Add(text); };
            void _error(string text) { stderr.Add(text); };
            StringReader _input = new("Giddyup");

            MiscUtils.GetSourcePath();
            var scriptFile = Path.Combine(MiscUtils.GetSourcePath(), "Files", "test_script.py");
            var code = Tools.RunScript(scriptFile, _print, _error, _input);

            Assert(stdout.Count == 5);
            Assert(stdout[3].Contains("print 3 -> Giddyup"));

            Assert(stderr.Count == 2);
            Assert(stderr[1].Contains("Error message 2!!!"));

            Assert(code == 999);
        }
    }
    public class TOOLS_MISC : TestSuite
    {
        public override void RunSuite()
        {
            Info("Test misc tools.");

            // public static string MarkdownToHtml(List<string> body, MarkdownMode mode, bool show)
            Tools.ShowReadme("NBagOfTricks");

            // TODO public static List<string> SniffBin(string fn, int limit = 100)

            // TODO public static (int ecode, string sret) ExecuteLuaCode(string scode)
        }
    }
}
