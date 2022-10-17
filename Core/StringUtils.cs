using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ephemera.NBagOfTricks
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
        /// Split by any of the delims but keep the delim.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="delims"></param>
        /// <returns></returns>
        public static List<string> SplitKeepDelims(this string s, string delims)
        {
            var parts = new List<string>();
            StringBuilder acc = new();

            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsWhiteSpace(s[i])) // skip ws
                {
                    if (delims.Contains(s[i])) // at delim
                    {
                        if (acc.Length > 0)
                        {
                            parts.Add(acc.ToString());
                            acc.Clear();
                        }
                        parts.Add(s[i].ToString());
                    }
                    else
                    {
                        acc.Append(s[i]);
                    }
                }
            }

            if (acc.Length > 0) // straggler?
            {
                parts.Add(acc.ToString());
            }

            return parts;
        }

        /// <summary>
        /// Specialized splitter, mainly for cmd line args.
        /// Input: 12345 "I HAVE SPACES" aaa bbb  "me too" ccc ddd "  and the last  "
        /// Output: 12345,I HAVE SPACES,aaa,bbb,me too,ccc,ddd,and the last
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Properly split values</returns>
        public static List<string> SplitQuotedString(this string text)
        {
            var ret = new List<string>();

            bool inquoted = false;
            string acc = "";

            foreach(char c in text)
            {
                if(c == '\"') // delimiter
                {
                    if(inquoted) // finish quoted
                    {
                        if(acc.Length > 0) { ret.Add(acc); }
                    }
                    else // start quoted
                    {
                        if (acc.Length > 0) { ret.AddRange(acc.SplitByToken(" ")); }
                    }

                    acc = "";
                    inquoted = !inquoted;
                }
                else // just accumulate
                {
                    acc += c;
                }
            }

            // Leftovers?
            if (acc.Length > 0)
            {
                if (inquoted) { ret.Add(acc); }
                else { ret.AddRange(acc.SplitByToken(" ")); }
            }

            return ret;
        }

        /// <summary>
        /// Because .NET framework doesn't have this.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool Contains(this string s, char c)
        {
            return s.IndexOf(c) != -1;
        }

        /// <summary>
        /// Gets the format specifier based upon the range of data.
        /// </summary>
        /// <param name="range">Data range</param>
        /// <returns>Format specifier</returns>
        public static string FormatSpecifier(float range)
        {
            string format = "";

            if (range >= 100)
            {
                format = "0;-0;0";
            }
            else if (range < 100 && range >= 10)
            {
                format = "0.0;-0.0;0";
            }
            else if (range < 10 && range >= 1)
            {
                format = "0.00;-0.00;0";
            }
            else if (range < 1)
            {
                format = "0.000;-0.000;0";
            }

            return format;
        }
    }
}
