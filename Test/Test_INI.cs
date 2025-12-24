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

            var irdr = new IniReader();
            irdr.DoFile(Path.Join(inputDir, "valid.ini"));
            var sections = irdr.GetSectionNames();
            UT_EQUAL(sections.Count, 5);

            UT_EQUAL(irdr.GetValues("test123").Count, 5);
            UT_EQUAL(irdr.GetValues("Some lists").Count, 2);
            var vv = irdr.GetValues("not here lists");
            UT_NULL(irdr.GetValues("not here lists"));

            // TODO more tests.
        }
    }
}
