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
        public static string InputDir = Path.Join(MiscUtils.GetSourcePath(), "Files");
        public static string OutputDir = Path.Join(MiscUtils.GetSourcePath(), "out");
        public static string SlogFile = Path.Combine(Program.OutputDir, "slog.log");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] _)
        {
            // Ensure paths.
            Directory.CreateDirectory(OutputDir);

            // Run pnut tests from cmd line.
            TestRunner runner = new(OutputFormat.Readable);
            var torun = new[] { "PBMP" };
            //var torun = new[] { "PNUT", "UTILS", "MMTEX", "JSON", "SLOG", "COLOR", "BMP", "INI", "MISC", "TOOLS" };
            //var torun = new[] { "UTILS", "MMTEX", "JSON", "SLOG", "COLOR", "BMP", "INI", "MISC", "TOOLS" };
            runner.RunSuites(torun);
            //runner.Context.OutputLines.ForEach(l => Console.WriteLine(l));
        }
    }
}
