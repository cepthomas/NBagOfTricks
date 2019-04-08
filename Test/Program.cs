﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBagOfTricks;
using NBagOfTricks.UI;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestHost w = new TestHost();
            //w.ShowDialog();

            TestRunner runner = new TestRunner(OutputFormat.Readable);
            //var cases = new[] { "PNUT", "ETC", "SM" };
            var cases = new[] { "SM" };
            runner.RunSuites(cases);
        }
    }
}
