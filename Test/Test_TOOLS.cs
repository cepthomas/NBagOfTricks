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
    public class TOOLS_MISC : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Test misc tools.");

            Tools.ShowReadme("NBagOfTricks");

            // void _print(string text) { Print(text, clr: _config.DebugColor); };
            // void _error(string text) { Print(text, clr: _config.ErrorColor); };

            // MiscUtils.GetSourcePath();
            // var scriptFile = Path.Combine(MiscUtils.GetSourcePath(), "Test", "test_script.py");
            // RunScript(scriptFile, _print, _error); TODO1

        }
    }
}
