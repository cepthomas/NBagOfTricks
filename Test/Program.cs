using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    class Program
    {
        static void Main(string[] _)
        {
            // Use pnut for automated lib tests.
            TestRunner runner = new TestRunner(OutputFormat.Readable);
            var cases = new[] { "SLOG_BASIC" };
            //var cases = new[] { "PNUT", "UTILS", "CMD", "MMTEX", "IPC", "TOOLS", "JSON", "SLOG" };
            runner.RunSuites(cases);
            File.WriteAllLines(@"..\..\out\test_out.txt", runner.Context.OutputLines);
        }
    }
}
