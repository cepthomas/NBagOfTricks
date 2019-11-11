using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NBagOfTricks.PNUT;
using NBagOfTricks.Utils;

// Tests for pnut itself.

namespace NBagOfTricks.Test
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

            UT_INFO("Suite tests core functions.");

            UT_INFO("Test UT_INFO. Visually inspect that this appears in the output.");

            UT_INFO("Test UT_INFO with parms.", int1, dbl2);

            UT_INFO("Should fail on UT_STR_EQUAL.");
            UT_EQUAL(str1, str2);

            // Should pass on UT_STR_EQUAL.
            UT_EQUAL(str2, "the mulberry bush");

            UT_EQUAL("".Count(), 0);

            UT_INFO("Should fail on UT_NOT_EQUAL.");
            UT_NOT_EQUAL(int1, 321);

            // Should pass on UT_NOT_EQUAL.
            UT_NOT_EQUAL(int2, int1);

            UT_INFO("Should fail on UT_LESS_OR_EQUAL.");
            UT_LESS_OR_EQUAL(int2, int1);

            // Should pass on UT_LESS_OR_EQUAL.
            UT_LESS_OR_EQUAL(int1, 321);

            // Should pass on UT_LESS_OR_EQUAL.
            UT_LESS_OR_EQUAL(int1, int2);

            UT_INFO("Should fail on UT_GREATER.");
            UT_GREATER(int1, int2);

            // Should pass on UT_GREATER.
            UT_GREATER(int2, int1);

            UT_INFO("Should fail on UT_GREATER_OR_EQUAL.");
            UT_GREATER_OR_EQUAL(int1, int2);

            // Should pass on UT_GREATER_OR_EQUAL.
            UT_GREATER_OR_EQUAL(int2, 987);

            // Should pass on UT_GREATER_OR_EQUAL.
            UT_GREATER_OR_EQUAL(int2, int1);

            // Should pass on UT_CLOSE.
            UT_CLOSE(dbl1, dbl2, dbl2 - dbl1 + dblTol);

            UT_INFO("Should fail on UT_CLOSE.");
            UT_CLOSE(dbl1, dbl1 - 2 * dblTol, dblTol);
        }
    }

    public class PNUT_TWO : TestSuite
    {
        public override void RunSuite()
        {
            int seed = 999;
            MathUtils.InitRand(seed);

            UT_PROPERTY("version", "xyz123");

            UT_PROPERTY("rand-seed", seed);

            int val1 = 1;
            int val2 = 2;

            UT_INFO("Visually inspect that this appears in the output with parm == 2.", val2);

            UT_EQUAL(val1 + val2, 3);

            UT_GREATER(val1, val2);

            UT_GREATER(val2, val1);
        }
    }

    public class ETC_NOT_PNUT : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Other stuff, could be in another file.");

            string str1 = MathUtils.RandStr(10);
            string str2 = str1.Clone() as string;
            UT_EQUAL(str1, str2);

            UT_EQUAL(str1, "Should fail");
            UT_INFO("Previous step should have failed.");

            UT_NOT_EQUAL(str1, "Should pass");

            UT_INFO("Tests various ...... functions.");

            // Cause an explosion.
            //UT_ASSERT(11, 22);
            //UT_INFO(Sub(0));
        }

        string Sub(int i)
        {
            int v = 100 / i;
            return v.ToString();
        }
    }
}
