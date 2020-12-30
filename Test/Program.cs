using System;
using System.Collections.Generic;
using System.Diagnostics;
using NBagOfTricks.UI;
using NBagOfTricks.PNUT;

namespace NBagOfTricks.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ///// Use pnut for automated lib tests.
            TestRunner runner = new TestRunner(OutputFormat.Readable);
            var cases = new[] { "UTILS" };
            //var cases = new[] { "UTILS", "PNUT", "SM", "CMD" };
            runner.RunSuites(cases);
            runner.Context.OutputLines.ForEach(l => Debug.WriteLine(l));

            ///// Use test host for debugging UI components.
            //TestHost w = new TestHost();
            //w.ShowDialog();
        }
    }
}
