using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ephemera.NBagOfTricks.PNUT;


// Tests for pnut itself.

namespace Ephemera.NBagOfTricks.Test
{
    public class PNUT_ONE : TestSuite
    {
        public override void RunSuite()
        {
            int int1 = 321;
            int int2 = 987;
            string str1 = "round and round";
            string str2 = "the mulberry bush";
            double dbl1 = 1.500;   
            double dbl2 = 1.600;
            double dblTol = 0.001;

            StopOnFail(false);

            Property("A string", "booga");
            Property("A number", 123.45);

            Info("Suite tests core functions.");

            Info("Inspect that there are 4 FAIL in the output.");

            Assert(str1 == str2, "Should FAIL");

            Assert(str2 == "the mulberry bush");

            Assert(int1 == 322, $"Should FAIL because int1 = {int1}");

            Assert(int2 < int1, "Should FAIL");

            Assert(int1 < int2);

            Assert(dbl1.IsClose(1.50002, dblTol));

            Assert(dbl1.IsClose(dbl2, dblTol), "Should FAIL");
        }
    }

    public class PNUT_STOP_ON_FAIL : TestSuite
    {
        public override void RunSuite()
        {
            int val1 = 1;

            StopOnFail(true);

            Info("Should appear in output");

            Assert(val1 == 3, "Should appear in output");

            Info("FAIL Should not appear in output");
        }
    }

    public class PNUT_FATAL2 : TestSuite
    {
        public override void RunSuite()
        {
            int val1 = 1;

            //StopOnFail(true);

            Info("Should appear in output");

            throw new Exception("Random error");

            Assert(val1 == 3, "FAIL Should not appear in output");
        }
    }

    public class PNUT_EXC : TestSuite
    {
        public override void RunSuite()
        {
            int ii = 99;
            int zero = 0;
            Exception? exThrown = null;

            StopOnFail(false);

            exThrown = Throws(typeof(DivideByZeroException), () =>
            {
                var zz = ii / zero;
            }, "FAIL Should not appear in output");
            Assert(exThrown == null);

            exThrown = Throws(typeof(ArgumentException), () =>
            {
                var zz = ii / zero;
            }, "Should appear in output");
            Assert(exThrown != null);
            Assert(exThrown!.Message == "Attempted to divide by zero.");

            exThrown = ThrowsNot(() =>
            {
                var zz = ii / 2;
            }, "FAIL Should not appear in output");
            Assert(exThrown == null);

            exThrown = ThrowsNot(() =>
            {
                var zz = ii / zero;
            }, "Should appear in output");
            Assert(exThrown != null);
            Assert(exThrown!.Message == "Attempted to divide by zero.");
        }
    }
}
