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
        /// Get the user app dir. Creates it if it doesn't exist.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="company">Optionally nest under company name.</param>
        /// <returns></returns>
        public static string GetAppDataDir(string appName, string company = "")
        {
            string localdir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string path = Path.Combine(localdir, company, appName);
            DirectoryInfo di = new(path);
            di.Create();
            return path;
        }

        /// <summary>
        /// Get the dir name of the caller's source file.
        /// </summary>
        /// <param name="callerPath"></param>
        /// <returns>Caller source dir.</returns>
        public static string GetSourcePath([CallerFilePath] string callerPath = "")
        {
            var dir = Path.GetDirectoryName(callerPath)!;
            return dir;
        }

        /// <summary>
        /// Display the application readme.
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static void ShowReadme(string appName)
        {
            string uri = $"https://github.com/cepthomas/{appName}/blob/main/README.md";
            var info = new ProcessStartInfo(uri) { UseShellExecute = true };
            var proc = new Process() { StartInfo = info };
            proc.Start();
        }

        /// <summary>
        /// Endian support.
        /// </summary>
        /// <param name="i">Number to fix.</param>
        /// <returns>Fixed number.</returns>
        public static UInt32 FixEndian(UInt32 i)
        {
            return BitConverter.IsLittleEndian ?
                ((i & 0xFF000000) >> 24) | ((i & 0x00FF0000) >> 8) | ((i & 0x0000FF00) << 8) | ((i & 0x000000FF) << 24) :
                i;
        }

        /// <summary>
        /// Endian support.
        /// </summary>
        /// <param name="i">Number to fix.</param>
        /// <returns>Fixed number.</returns>
        public static UInt16 FixEndian(UInt16 i)
        {
            return BitConverter.IsLittleEndian ?
                (UInt16)(((i & 0xFF00) >> 8) | ((i & 0x00FF) << 8)) :
                i;
        }
        #endregion

        // #region Time
        // /// <summary>
        // /// Convert time to TimeSpan.
        // /// </summary>
        // /// <param name="sec">Time in seconds.</param>
        // /// <returns>TimeSpan</returns>
        // public static TimeSpan SecondsToTimeSpan(double sec)
        // {
        //     var (integral, fractional) = MathUtils.SplitDouble(sec);
        //     int isec = (int)integral;
        //     int imsec = (int)(fractional * 1000);
        //     TimeSpan ts = new(0, 0, 0, isec, imsec);
        //     return ts;
        // }

        // /// <summary>
        // /// Convert TimeSpan to time.
        // /// </summary>
        // /// <param name="ts"></param>
        // /// <returns>Time in seconds.</returns>
        // public static double TimeSpanToSeconds(TimeSpan ts)
        // {
        //     return ts.TotalSeconds;
        // }
        // #endregion

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
        /// Invoke helper, maybe.
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
            if(source is not null)
            {
                var index = 0;
                foreach (var element in source)
                {
                    action(element, index++);
                }
            }
        }
        #endregion
    }
}
