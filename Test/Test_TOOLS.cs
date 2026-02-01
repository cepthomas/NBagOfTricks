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
            UT_INFO("Test RunScript()."); //TODO1 more - stderr, stdin

            var stdout = new List<string>();
            var stderr = new List<string>();

            // public static int RunScript(string fn, Action<string> output, Action<string> error, TextReader? input = null)
            void _print(string text)
            {
                stdout.Add(text);
            };

            void _error(string text)
            {
                stderr.Add(text);
            };

            MiscUtils.GetSourcePath();
            var scriptFile = Path.Combine(MiscUtils.GetSourcePath(), "Files", "test_script.py");
            Tools.RunScript(scriptFile, _print, _error);
        }
    }
    public class TOOLS_MISC : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Test misc tools.");

            // public static string MarkdownToHtml(List<string> body, MarkdownMode mode, bool show)
            Tools.ShowReadme("NBagOfTricks");

            // TODO public static List<string> SniffBin(string fn, int limit = 100)

            // TODO public static (int ecode, string sret) ExecuteLuaCode(string scode)
        }
    }
}
