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
            bool doTests = true;

            if (doTests)
            {
                // Make sure out path exists.
                var outPath = MiscUtils.GetSourcePath();

                // Run pnut tests from cmd line.
                TestRunner runner = new(OutputFormat.Readable);
                var torun = new[] { "PNUT", "JSON" };
                //var torun = new[] { "PNUT", "UTILS", "MMTEX", "JSON", "SLOG", "COLOR", "BMP", "INI", "MISC", "TOOLS" };
                runner.RunSuites(torun);
                //runner.Context.OutputLines.ForEach(l => Console.WriteLine(l));
            }
            else
            {
                Console.WriteLine("Hello");
                Debug.WriteLine($"{DateTime.Now} Call hide");
                ConsoleUtils.Hide();
                //Console.WriteLine("Should be gone");  boom
                Thread.Sleep(2000);
                Debug.WriteLine($"{DateTime.Now} Exiting");
            }

            Environment.Exit(0);
        }
    }
}
