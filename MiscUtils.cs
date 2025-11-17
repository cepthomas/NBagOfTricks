using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;


namespace Ephemera.NBagOfTricks
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
            string localdir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
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
        /// Endian support.
        /// </summary>
        /// <param name="i">Number to fix.</param>
        /// <returns>Fixed number.</returns>
        public static uint FixEndian(uint i)
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
        public static ushort FixEndian(ushort i)
        {
            return BitConverter.IsLittleEndian ?
                (ushort)(((i & 0xFF00) >> 8) | ((i & 0x00FF) << 8)) :
                i;
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
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var element in source)
            {
                action(element);
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
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var index = 0;
            foreach (var element in source)
            {
                action(element, index++);
            }
        }
        #endregion
    }
}
