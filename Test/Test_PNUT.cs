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

            Info("Inspect that there are 3 FAIL in the output.");

            Assert(str1 != str2, "Should FAIL");

            Assert(str2 == "the mulberry bush");

            Assert(int1 == 322, "Should FAIL");

            Assert(int2 < int1, "Should FAIL");

            Assert(int1 < int2);

            // close
            Assert(Math.Abs(dbl1 - dbl2) > dblTol);
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

            StopOnFail(false);

            Throws(typeof(DivideByZeroException), () =>
            {
                var zz = ii / zero;
            }, "FAIL Should not appear in output");

            Throws(typeof(ArgumentException), () =>
            {
                var zz = ii / zero;
            }, "Should appear in output");

            ThrowsNot(() =>
            {
                var zz = ii / 2;
            }, "FAIL Should not appear in output");

            ThrowsNot(() =>
            {
                var zz = ii / zero;
            }, "Should appear in output");
        }
    }
}
