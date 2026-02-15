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
using System.Drawing.Imaging;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class INI_MAIN : TestSuite
    {
        public override void RunSuite()
        {
            Info("Test ini reader.");

            var inputDir = Path.Join(MiscUtils.GetSourcePath(), "Files");
            // var outputDir = Path.Join(MiscUtils.GetSourcePath(), "out");

            var irdr = new IniReader();
            irdr.ParseFile(Path.Join(inputDir, "valid.ini"));
            var sections = irdr.GetSectionNames();
            Assert(sections.Count == 6);

            Assert(irdr.GetValues("test123").Count == 5);
            Assert(irdr.GetValues("Some lists").Count == 2);

            Throws(typeof(InvalidOperationException), () =>
            {
                irdr.GetValues("not here lists");
            });

            // TODO more tests.
        }
    }
}
