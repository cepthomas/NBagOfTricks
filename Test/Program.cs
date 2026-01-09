using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            // Make sure out path exists.
             var outPath = Path.Join(MiscUtils.GetSourcePath(), "out");
            DirectoryInfo di = new(outPath);
            di.Create();

            // Run pnut tests from cmd line.
            TestRunner runner = new(OutputFormat.Readable);
            var cases = new[] { "SLOG_OPTS" };
            //var cases = new[] { "UTILS", "MMTEX", "JSON", "SLOG", "COLOR", "BMP", "INI", "MISC" };
            //var cases = new[] { "PNUT", "UTILS", "MMTEX", "JSON", "SLOG", "COLOR", "BMP", "INI", "MISC" };
            runner.RunSuites(cases);

            //var fn = Path.Combine(MiscUtils.GetSourcePath(), "out", "pnut_out.txt");
            //File.WriteAllLines(fn, runner.Context.OutputLines);

            //runner.Context.OutputLines.ForEach(l => Console.WriteLine(l));

            Environment.Exit(0);
        }
    }
}
