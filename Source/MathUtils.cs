using System;
using System.Collections.Generic;
using System.Linq;


namespace NBagOfTricks
{
    /// <summary>
    /// Static math utility functions.
    /// </summary>
    public static class MathUtils
    {
        static Random _rand = new Random();

        /// <summary>
        /// Seed the randomizer.
        /// </summary>
        /// <param name="seed"></param>
        public static void InitRand(int seed)
        {
            _rand = new Random(seed);
        }

        /// <summary>
        /// Get a random alphanumeric string.
        /// </summary>
        /// <param name="num">String length.</param>
        /// <returns></returns>
        public static string RandStr(int num)
        {
            char[] chars = new char[num];
            for (int i = 0; i < num; i++)
            {
                chars[i] = (char)_rand.Next(32, 126); // printables
            }

            return new string(chars);
        }

        /// <summary>
        /// Get the next random integer in the range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandRange(int min, int max)
        {
            return _rand.Next(min, max);
        }

        /// <summary>
        /// Get the next random double in the range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double RandRange(double min, double max)
        {
            double dr = _rand.NextDouble();
            double v = min + (max - min) * dr;
            return v;
        }

        /// <summary>
        /// Compare two doubles "close enough".
        /// </summary>
        public static bool IsClose(this double t1, double t2, double tol = 0.000001)
        {
            return Math.Abs(t2 - t1) < tol;
        }

        /// <summary>
        /// Split a double into two parts: each side of the dp.
        /// </summary>
        /// <param name="val"></param>
        /// <returns>tuple of integral and fractional</returns>
        public static (double integral, double fractional) SplitDouble(double val)
        {
            double integral = Math.Truncate(val);
            double fractional = val - integral;
            return (integral, fractional);
        }

        /// <summary>Conversion.</summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        /// <summary>Conversion.</summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double angle)
        {
            return angle * 180.0 / Math.PI;
        }

        /// <summary>
        /// Remap a value to new coordinates.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="start1"></param>
        /// <param name="stop1"></param>
        /// <param name="start2"></param>
        /// <param name="stop2"></param>
        /// <returns></returns>
        public static double Map(double val, double start1, double stop1, double start2, double stop2)
        {
            return start2 + (stop2 - start2) * (val - start1) / (stop1 - start1);
        }

        /// <summary>
        /// Remap a value to new coordinates.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="start1"></param>
        /// <param name="stop1"></param>
        /// <param name="start2"></param>
        /// <param name="stop2"></param>
        /// <returns></returns>
        public static int Map(int val, int start1, int stop1, int start2, int stop2)
        {
            return start2 + (stop2 - start2) * (val - start1) / (stop1 - start1);
        }

        /// <summary>
        /// Calculate a Standard Deviation based on a List of doubles.
        /// </summary>
        /// <param name="inputArray">List of doubles</param>
        /// <returns>Double value of the Standard Deviation</returns>
        public static double StandardDeviation(List<double> inputArray)
        {
            double sd = double.NaN;

            if (inputArray.Count > 1)
            {
                double sumOfSquares = SumOfSquares(inputArray);
                sd = sumOfSquares / (inputArray.Count - 1);
            }
            else // Divide by Zero
            {
                sd = double.NaN;
            }
            if (sd < 0) // Square Root of Neg Number
            {
                sd = double.NaN;
            }

            sd = Math.Sqrt(sd); // Square Root of sd
            return sd;
        }

        /// <summary>
        /// Calculate a Sum of Squares given a List of doubles.
        /// </summary>
        /// <param name="inputArray">List of doubles</param>
        /// <returns>Double value of the Sum of Squares</returns>
        public static double SumOfSquares(List<double> inputArray)
        {
            double mean;
            double sumOfSquares;
            sumOfSquares = 0;

            mean = inputArray.Average();

            foreach (double v in inputArray)
            {
                sumOfSquares += Math.Pow((v - mean), 2);
            }
            return sumOfSquares;
        }

        /// <summary>
        /// Generates normally distributed numbers.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="mean">Mean</param>
        /// <param name="sigma">Sigma</param>
        /// <returns></returns>
        public static double NextGaussian(Random r, double mean = 0, double sigma = 1)
        {
            var u1 = r.NextDouble();
            var u2 = r.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            var randNormal = mean + sigma * randStdNormal;
            return randNormal;
        }

        /// <summary>
        /// Bounds limits a value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Constrain(double val, double min, double max)
        {
            val = Math.Max(val, min);
            val = Math.Min(val, max);
            return val;
        }

        /// <summary>
        /// Bounds limits a value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Constrain(int val, int min, int max)
        {
            val = Math.Max(val, min);
            val = Math.Min(val, max);
            return val;
        }
    }
}
