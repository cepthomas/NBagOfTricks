using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ephemera.NBagOfTricks.PNUT;
using Microsoft.CodeAnalysis.CSharp.Syntax;


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

            UT_STOP_ON_FAIL(false);

            UT_INFO("Suite tests core functions.");

            UT_INFO("Visually inspect that there are 6 failures in the output.");

            UT_INFO("Test UT_INFO with args", int1, dbl2);

            UT_INFO("Next test should fail on UT_STR_EQUAL.");
            UT_EQUAL(str1, str2);

            // Next test should pass on UT_STR_EQUAL.
            UT_EQUAL(str2, "the mulberry bush");

            UT_EQUAL("".Length, 0);

            UT_INFO("Next test should fail on UT_NOT_EQUAL.");
            UT_NOT_EQUAL(int1, 321);

            // Next test should pass on UT_NOT_EQUAL.
            UT_NOT_EQUAL(int2, int1);

            UT_INFO("Next test should fail on UT_LESS_OR_EQUAL.");
            UT_LESS_OR_EQUAL(int2, int1);

            // Next test should pass on UT_LESS_OR_EQUAL.
            UT_LESS_OR_EQUAL(int1, int1);

            // Next test should pass on UT_LESS_OR_EQUAL.
            UT_LESS_OR_EQUAL(int1, int2);

            UT_INFO("Next test should fail on UT_GREATER.");
            UT_GREATER(int1, int2);

            // Next test should pass on UT_GREATER.
            UT_GREATER(int2, int1);

            UT_INFO("Next test should fail on UT_GREATER_OR_EQUAL.");
            UT_GREATER_OR_EQUAL(int1, int2);

            // Next test should pass on UT_GREATER_OR_EQUAL.
            UT_GREATER_OR_EQUAL(int2, int2);

            // Next test should pass on UT_GREATER_OR_EQUAL.
            UT_GREATER_OR_EQUAL(int2, int1);

            // Next test should pass on UT_CLOSE.
            UT_CLOSE(dbl1, dbl2, dbl2 - dbl1 + dblTol);

            UT_INFO("Next test should fail on UT_CLOSE.");
            UT_CLOSE(dbl1, dbl1 - 2 * dblTol, dblTol);
        }
    }

    public class PNUT_FATAL1 : TestSuite
    {
        public override void RunSuite()
        {
            int val1 = 1;

            UT_STOP_ON_FAIL(true);

            UT_INFO("Visually inspect that this appears in the output");

            UT_EQUAL(val1, 3);

            UT_INFO("Visually inspect that this does not appear in the output");
        }
    }

    public class PNUT_FATAL2 : TestSuite
    {
        public override void RunSuite()
        {
            int val1 = 1;

            UT_STOP_ON_FAIL(true);

            UT_INFO("Visually inspect that this appears in the output");

            throw new Exception("Random error");

#pragma warning disable CS0162 // Unreachable code detected
            UT_EQUAL(val1, 3);
#pragma warning restore CS0162 // Unreachable code detected

            UT_INFO("Visually inspect that this does not appear in the output");
        }
    }

    public class PNUT_TWO : TestSuite
    {
        public override void RunSuite()
        {
            int seed = 999;
            MathUtils.InitRand(seed);

            UT_STOP_ON_FAIL(false);

            UT_PROPERTY("version", "xyz123");

            UT_PROPERTY("rand-seed", seed);

            int val1 = 1;
            int val2 = 2;

            UT_INFO("Visually inspect that this appears in the output with arg", val2);

            UT_EQUAL(val1 + val2, 3);

            UT_GREATER(val1, val2);

            UT_GREATER(val2, val1);
        }
    }

    public class PNUT_EXC : TestSuite
    {
        public override void RunSuite()
        {
            int ii = 99;
            int zero = 0;

            // Next test should pass on UT_THROWS.
            UT_THROWS(typeof(DivideByZeroException), () =>
            {
                var zz = ii / zero;
            });

            UT_INFO("Next test should fail on UT_THROWS.");
            UT_THROWS(typeof(ArgumentException), () =>
            {
                var zz = ii / zero;
            });

            // Next test should pass on UT_THROWS_NOT.
            UT_THROWS_NOT(() =>
            {
                var zz = ii / 2;
            });

            UT_INFO("Next test should fail on UT_THROWS_NOT.");
            UT_THROWS_NOT(() =>
            {
                var zz = ii / zero;
            });
        }
    }

    public class ETC_NOT_PNUT : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Other stuff, could be in another file.");

            string str1 = MathUtils.RandStr(10);
            string? str2 = str1.Clone() as string;
            UT_EQUAL(str1, str2!);

            UT_EQUAL(str1, "Should fail");
            UT_INFO("Previous step should have failed.");

            UT_NOT_EQUAL(str1, "Should pass");

            UT_INFO("Test various ...... functions.");

            // Cause an explosion.
            //UT_ASSERT(11, 22);
            //UT_INFO($"{100/0}");
        }
    }
}
