using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestRunner runner = new TestRunner(OutputFormat.Readable);
            var cases = new[] { "PNUT", "ETC", "SM" };
            runner.RunSuites(cases);
        }
    }
}
