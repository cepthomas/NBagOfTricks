using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBagOfTricks.UI;
using NBagOfTricks.PNUT;


namespace NBagOfTricks.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use test host for debugging UI components.
            TestHost w = new TestHost();
            w.RunTests();
            w.ShowDialog();
        }
    }
}
