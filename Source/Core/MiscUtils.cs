using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;


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
            Version? ver = Assembly.GetCallingAssembly().GetName().Version;
            return ver is null ? "unknown" : $"{ver.Major}.{ver.Minor}.{ver.Build}";
        }

        /// <summary>
        /// Get the user app dir.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="company">Optional nest under company name.</param>
        /// <returns></returns>
        public static string GetAppDataDir(string appName, string company = "")
        {
            string localdir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localdir, company, appName);
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

        /// <summary>
        /// Get the dir name of the caller source file.
        /// </summary>
        /// <param name="callerPath"></param>
        /// <returns>Caller source dir.</returns>
        public static string GetSourcePath([CallerFilePath] string callerPath = "")
        {
            var dir = Path.GetDirectoryName(callerPath)!;
            return dir;
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
            var (integral, fractional) = MathUtils.SplitDouble(sec);
            int isec = (int)integral;
            int imsec = (int)(fractional * 1000);
            TimeSpan ts = new(0, 0, 0, isec, imsec);
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

        #region Misc extensions
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
            if (obj is not null)
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
            if(source is not null)
            {
                foreach (var element in source)
                {
                    action(element);
                }
            }
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
            var index = 0;
            foreach (var element in source)
            {
                action(element, index++);
            }
        }
        #endregion
    }
}
