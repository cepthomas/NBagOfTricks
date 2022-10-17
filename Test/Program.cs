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
            var cases = new[] { "UTILS_SETTINGS" };
            //var cases = new[] { "PNUT", "UTILS", "CMD", "MMTEX", "IPC", "TOOLS", "JSON", "SLOG" };
            runner.RunSuites(cases);
            File.WriteAllLines(@"..\..\out\test_out.txt", runner.Context.OutputLines);

            //// Run pnut tests from ui host.
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new TestHost());
        }
    }
}
