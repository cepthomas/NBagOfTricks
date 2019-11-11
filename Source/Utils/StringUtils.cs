using System;
using System.Collections.Generic;


namespace NBagOfTricks.Utils
{
   /// <summary>
   /// Misc string extension methods.
   /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Test for integer string.
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static bool IsInteger(this string sourceString)
        {
            bool isint = true;
            sourceString.ForEach(c => isint &= char.IsNumber(c));
            return isint;
        }

        /// <summary>
        /// Test for float string.
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static bool IsFloat(this string sourceString)
        {
            bool isint = true;
            sourceString.ForEach(c => isint &= (char.IsNumber(c) || c == '.'));
            return isint;
        }

        /// <summary>
        /// Test for alpha string.
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static bool IsAlpha(this string sourceString)
        {
            bool isalpha = true;
            sourceString.ForEach(c => isalpha &= char.IsLetter(c));
            return isalpha;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsReadable(byte b)
        {
            return b >= 32 && b <= 126;
        }

        /// <summary>
        /// Returns the rightmost characters of a string based on the number of characters specified.
        /// </summary>
        /// <param name="str">The source string to return characters from.</param>
        /// <param name="numChars">The number of rightmost characters to return.</param>
        /// <returns>The rightmost characters of a string.</returns>
        public static string Right(this string str, int numChars)
        {
            numChars = MathUtils.Constrain(numChars, 0, str.Length);
            return str.Substring(str.Length - numChars, numChars);
        }

        /// <summary>
        /// Returns the leftmost number of chars in the string.
        /// </summary>
        /// <param name="str">The source string .</param>
        /// <param name="numChars">The number of characters to get from the source string.</param>
        /// <returns>The leftmost number of characters to return from the source string supplied.</returns>
        public static string Left(this string str, int numChars)
        {
            numChars = MathUtils.Constrain(numChars, 0, str.Length);
            return str.Substring(0, numChars);
        }

        /// <summary>
        /// Splits a tokenized (delimited) string into its parts and optionally trims whitespace.
        /// </summary>
        /// <param name="text">The string to split up.</param>
        /// <param name="tokens">The char token(s) to split by.</param>
        /// <param name="trim">Remove whitespace at each end.</param>
        /// <returns>Return the parts as a list.</returns>
        public static List<string> SplitByTokens(this string text, string tokens, bool trim = true)
        {
            var ret = new List<string>();
            var list = text.Split(tokens.ToCharArray(), trim ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            list.ForEach(s => ret.Add(trim ? s.Trim() : s));
            return ret;
        }

        /// <summary>
        /// Splits a tokenized (delimited) string into its parts and optionally trims whitespace.
        /// </summary>
        /// <param name="text">The string to split up.</param>
        /// <param name="splitby">The string to split by.</param>
        /// <param name="trim">Remove whitespace at each end.</param>
        /// <returns>Return the parts as a list.</returns>
        public static List<string> SplitByToken(this string text, string splitby, bool trim = true)
        {
            var ret = new List<string>();
            var list = text.Split(new string[] { splitby }, trim ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            list.ForEach(s => ret.Add(trim ? s.Trim() : s));
            return ret;
        }

        /// <summary>
        /// Update the MRU.
        /// </summary>
        /// <param name="mruList">The MRU.</param>
        /// <param name="newVal">New value(s) to perhaps insert.</param>
        /// <param name="mruSize">Optional max size.</param>
        public static void UpdateMru(this List<string> mruList, string newVal, int mruSize = 20)
        {
            // First check if it's already in there.
            for (int i = 0; i < mruList.Count; i++)
            {
                if (newVal == mruList[i])
                {
                    // Remove from current location so we can stuff it back in later.
                    mruList.Remove(mruList[i]);
                }
            }

            // Insert at the front and trim the tail.
            mruList.Insert(0, newVal);
            while (mruList.Count > mruSize)
            {
                mruList.RemoveAt(mruList.Count - 1);
            }
        }
    }
}
