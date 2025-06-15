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
            UT_INFO("Test various util functions.");

            string input = "12345 \"I HAVE SPACES\" aaa bbb  \"me too\" ccc ddd \"  and the last  \"";
            // Output: 12345,I HAVE SPACES,aaa,bbb,me too,ccc,ddd,and the last

            var splits = input.SplitQuotedString();

            UT_EQUAL(splits.Count, 8);
            UT_EQUAL(splits[0], "12345");
            UT_EQUAL(splits[1], "I HAVE SPACES");
            UT_EQUAL(splits[2], "aaa");
            UT_EQUAL(splits[3], "bbb");
            UT_EQUAL(splits[4], "me too");
            UT_EQUAL(splits[5], "ccc");
            UT_EQUAL(splits[6], "ddd");
            UT_EQUAL(splits[7], "  and the last  ");

            input = " \"aaa ttt uuu\" 84ss \"  dangling quote  ";
            splits = input.SplitQuotedString();

            UT_EQUAL(splits.Count, 3);
            UT_EQUAL(splits[0], "aaa ttt uuu");
            UT_EQUAL(splits[1], "84ss");
            UT_EQUAL(splits[2], "  dangling quote  ");
        }
    }

    public class UTILS_MISC : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Test misc utils.");

            var dir = MiscUtils.GetAppDataDir("Test");
            UT_EQUAL(dir, @"C:\Users\cepth\AppData\Roaming\Test");

            dir = MiscUtils.GetAppDataDir("Bar", "Test");
            UT_EQUAL(dir, @"C:\Users\cepth\AppData\Roaming\Test\Bar");

            Tools.ShowReadme("NBagOfTricks");
        }
    }
}
