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
            // Run pnut tests from cmd line.
            TestRunner runner = new(OutputFormat.Readable);
            var cases = new[] { "BMP" };
             //var cases = new[] { "PNUT", "UTILS", "MMTEX", "JSON", "SLOG", "COLOR" };
            runner.RunSuites(cases);

            //var fn = Path.Combine(MiscUtils.GetSourcePath(), "out", "pnut_out.txt");
            //File.WriteAllLines(fn, runner.Context.OutputLines);

            //runner.Context.OutputLines.ForEach(l => Console.WriteLine(l));

            // // Run pnut tests from ui host.
            // Application.SetHighDpiMode(HighDpiMode.SystemAware);
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new TestHost());

            Environment.Exit(0);
        }
    }
}
