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
            UT_STOP_ON_FAIL(true);

            UT_INFO("Test RunScript().");

            var stdout = new List<string>();
            var stderr = new List<string>();

            void _print(string text) { stdout.Add(text); };
            void _error(string text) { stderr.Add(text); };
            StringReader _input = new("Giddyup");

            MiscUtils.GetSourcePath();
            var scriptFile = Path.Combine(MiscUtils.GetSourcePath(), "Files", "test_script.py");
            var code = Tools.RunScript(scriptFile, _print, _error, _input);

            UT_EQUAL(stdout.Count, 5);
            UT_STRING_CONTAINS(stdout[3], "print 3 -> Giddyup");

            UT_EQUAL(stderr.Count, 2);
            UT_STRING_CONTAINS(stderr[1], "Error message 2!!!");

            UT_EQUAL(code, 999);
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
