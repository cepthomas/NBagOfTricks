using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] _)
        {
            // Ensure paths.
            var outputDir = Path.Join(MiscUtils.GetSourcePath(), "out");
            Directory.CreateDirectory(outputDir);

            // Run pnut tests from cmd line.
            TestRunner runner = new(OutputFormat.Readable);
            //var torun = new[] { "PNUT", "JSON" };
            //var torun = new[] { "PNUT", "UTILS", "MMTEX", "JSON", "SLOG", "COLOR", "BMP", "INI", "MISC", "TOOLS" };
            var torun = new[] { "UTILS", "MMTEX", "JSON", "SLOG", "COLOR", "BMP", "INI", "MISC", "TOOLS" };
            runner.RunSuites(torun);
            //runner.Context.OutputLines.ForEach(l => Console.WriteLine(l));
        }
    }
}
