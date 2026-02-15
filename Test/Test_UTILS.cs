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


// Tests for the *Utils.cs components.

namespace Ephemera.NBagOfTricks.Test
{
    public class UTILS_EXTENSIONS : TestSuite
    {
        public override void RunSuite()
        {
            Info("Test various util functions.");

            string input = "12345 \"I HAVE SPACES\" aaa bbb  \"me too\" ccc ddd \"  and the last  \"";
            // Output: 12345,I HAVE SPACES,aaa,bbb,me too,ccc,ddd,and the last

            var splits = input.SplitQuotedString();

            Assert(splits.Count == 8);
            Assert(splits[0] == "12345");
            Assert(splits[1] == "I HAVE SPACES");
            Assert(splits[2] == "aaa");
            Assert(splits[3] == "bbb");
            Assert(splits[4] == "me too");
            Assert(splits[5] == "ccc");
            Assert(splits[6] == "ddd");
            Assert(splits[7] == "  and the last  ");

            input = " \"aaa ttt uuu\" 84ss \"  dangling quote  ";
            splits = input.SplitQuotedString();

            Assert(splits.Count == 3);
            Assert(splits[0] == "aaa ttt uuu");
            Assert(splits[1] == "84ss");
            Assert(splits[2] == "  dangling quote  ");
        }
    }

    public class UTILS_MISC : TestSuite
    {
        public override void RunSuite()
        {
            Info("Test misc utils.");

            var dir = MiscUtils.GetAppDataDir("Test");
            Assert(dir.Contains(@"\AppData\Roaming\Test"));

            dir = MiscUtils.GetAppDataDir("Bar", "Test");
            Assert(dir.Contains(@"\AppData\Roaming\Test\Bar"));

            Tools.ShowReadme("NBagOfTricks");
        }
    }
}
