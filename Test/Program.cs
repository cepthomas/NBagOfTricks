using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NBagOfTricks;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    class Program
    {
        static void Main(string[] _)
        {
            // Use pnut for automated lib tests.
            TestRunner runner = new TestRunner(OutputFormat.Readable);
            var cases = new[] { "IPC" };
            //var cases = new[] { "UTILS", "CMD", "MMTEX" };
            //var cases = new[] { "PNUT", "UTILS", "CMD", "MMTEX" };
            runner.RunSuites(cases);
            File.WriteAllLines(@"test_out.txt", runner.Context.OutputLines);
        }
    }
}
