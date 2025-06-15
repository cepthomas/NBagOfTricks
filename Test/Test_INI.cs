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
using System.Drawing.Imaging;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class INI_MAIN : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Test ini reader.");

            var inputDir = Path.Join(MiscUtils.GetSourcePath(), "Files");
            // var outputDir = Path.Join(MiscUtils.GetSourcePath(), "out");

            var irdr = new IniReader(Path.Join(inputDir, "valid.ini"));
            UT_EQUAL(irdr.Contents.Count, 5);
            UT_EQUAL(irdr.Contents["test123"].Values.Count, 5);
            UT_EQUAL(irdr.Contents["Some lists"].Values.Count, 2);

            // TODO more tests.
        }
    }
}
