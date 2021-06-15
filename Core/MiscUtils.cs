﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;


namespace NBagOfTricks
{
    /// <summary>
    /// Static general utility functions.
    /// </summary>
    public static class MiscUtils
    {
        #region System utils
        /// <summary>
        /// Returns a string with the application version information.
        /// </summary>
        public static string GetVersionString()
        {
            Version ver = typeof(MiscUtils).Assembly.GetName().Version;
            return $"{ver.Major}.{ver.Minor}.{ver.Build}";
        }

        /// <summary>
        /// Get the user app dir.
        /// </summary>
        /// <returns></returns>
        public static string GetAppDataDir(string appName)
        {
            string localdir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localdir, appName);
        }

        /// <summary>
        /// Get the executable dir.
        /// </summary>
        /// <returns></returns>
        public static string GetExeDir()
        {
            string sdir = Environment.CurrentDirectory;
            return sdir;
        }
        #endregion

        #region Time
        /// <summary>
        /// Convert time to TimeSpan.
        /// </summary>
        /// <param name="sec">Time in seconds.</param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan SecondsToTimeSpan(double sec)
        {
            var v = MathUtils.SplitDouble(sec);
            int isec = (int)(v.integral);
            int imsec = (int)(v.fractional * 1000);
            TimeSpan ts = new TimeSpan(0, 0, 0, isec, imsec);
            return ts;
        }

        /// <summary>
        /// Convert TimeSpan to time.
        /// </summary>
        /// <param name="ts"></param>
        /// <returns>Time in seconds.</returns>
        public static double TimeSpanToSeconds(TimeSpan ts)
        {
            return ts.TotalSeconds;
        }
        #endregion

        #region File utils
        /// <summary>
        /// Reports non-ascii characters in a file. UTF-8 only.
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static List<string> SniffBin(string fn)
        {
            List<string> res = new List<string>();

            using (var fo = File.OpenRead(fn))
            {
                int i = 0;
                int row = 1;
                int col = 1;
                bool done = false;

                while (!done)
                {
                    int b = fo.ReadByte();

                    switch (b)
                    {
                        case -1:
                            done = true;
                            break;

                        case '\n':
                            // Ignore ok binaries
                            row++;
                            col = 1;
                            break;

                        case '\r':
                        case '\t':
                            // Ignore ok binaries
                            break;

                        default:
                            //‭ Everything else is binary.
                            if (b < 32 || b > 126) //32  SPACE  126  ~
                            {
                                //done = true;
                                res.Add($"row:{row} col:{col} val:{i}({i:X}) b:{b:X}");
                            }
                            col++;
                            break;
                    }

                    i++;
                }
            }

            return res;
        }
        #endregion

        #region Misc extensions
        /// <summary>Rudimentary C# source code formatter to make generated files somewhat readable.</summary>
        /// <param name="src">Lines to prettify.</param>
        /// <returns>Formatted lines.</returns>
        public static List<string> FormatSourceCode(List<string> src)
        {
            List<string> fmt = new List<string>();
            int indent = 0;

            src.ForEach(s =>
            {
                if (s.StartsWith("{") && !s.Contains("}"))
                {
                    fmt.Add(new string(' ', indent * 4) + s);
                    indent++;
                }
                else if (s.StartsWith("}") && indent > 0)
                {
                    indent--;
                    fmt.Add(new string(' ', indent * 4) + s);
                }
                else
                {
                    fmt.Add(new string(' ', indent * 4) + s);
                }
            });

            return fmt;
        }

        /// <summary>
        /// Perform a blind deep copy of an object. The class must be marked as [Serializable] in order for this to work.
        /// There are many ways to do this: http://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-an-object-in-net-c-specifically/11308879
        /// The binary serialization is apparently slower but safer. Feel free to reimplement with a better way.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Get a subset of an array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] Subset<T>(this T[] source, int start, int length)
        {
            T[] subset = new T[length];
            Array.Copy(source, start, subset, 0, length);
            return subset;
        }

        /// <summary>
        /// Invoke helper. See http://stackoverflow.com/a/29497681
        /// Usage:
        /// progressBar1.InvokeIfRequired(o => 
        /// {
        ///     o.Style = ProgressBarStyle.Marquee;
        ///     o.MarqueeAnimationSpeed = 40;
        /// });
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        public static void InvokeIfRequired<T>(this T obj, InvokeIfRequiredDelegate<T> action) where T : ISynchronizeInvoke
        {
            if (obj != null)
            {
                if (obj.InvokeRequired)
                {
                    obj.Invoke(action, new object[] { obj });
                }
                else
                {
                    action(obj);
                }
            }
        }
        public delegate void InvokeIfRequiredDelegate<T>(T obj) where T : ISynchronizeInvoke;
        #endregion

        #region Extensions borrowed from MoreLinq
        /// <summary>
        /// Immediately executes the given action on each element in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence</typeparam>
        /// <param name="source">The sequence of elements</param>
        /// <param name="action">The action to execute on each element</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var element in source)
                action(element);
        }

        /// <summary>
        /// Immediately executes the given action on each element in the source sequence.
        /// Each element's index is used in the logic of the action.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence</typeparam>
        /// <param name="source">The sequence of elements</param>
        /// <param name="action">The action to execute on each element; the second parameter
        /// of the action represents the index of the source element.</param>

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var index = 0;
            foreach (var element in source)
                action(element, index++);
        }
        #endregion
    }
}
