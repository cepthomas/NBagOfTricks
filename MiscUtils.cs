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

        /// <summary>
        /// Create a new clean filename for export. Creates path if it doesn't exist.
        /// </summary>
        /// <param name="path">Export path</param>
        /// <param name="baseFn">Root of the new file name</param>
        /// <param name="mod">Modifier</param>
        /// <param name="ext">File extension</param>
        /// <returns></returns>
        public static string MakeExportFileName(string path, string baseFn, string mod, string ext)
        {
            string name = Path.GetFileNameWithoutExtension(baseFn);

            // Clean the file name.
            name = name.Replace('.', '-').Replace(' ', '_');
            mod = mod == "" ? "default" : mod.Replace(' ', '_');
            var newfn = Path.Join(path, $"{name}_{mod}.{ext}");
            return newfn;
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


        /// <summary>How to render</summary>
        public enum MarkdownMode
        {
            Simple,     // Plain page with TOC at the top.
            DarkApi,    // Dark page with TOC on the left.
            LightApi    // Light page with TOC on the right.
        }

        /// <summary>
        /// Convert list of markdown lines to html.
        /// </summary>
        /// <param name="body">The md text.</param>
        /// <param name="mode">What flavor.</param>
        /// <param name="show">If true open in browser.</param>
        /// <returns>Valid html.</returns>
        public static string MarkdownToHtml(List<string> body, MarkdownMode mode, bool show) // TODO find a better home
        {
            // Put it together.
            var mdText = new List<string>();

            switch (mode)
            {
                case MarkdownMode.Simple:
                    mdText.Add($"<style>body {{ background-color: LightYellow; font-family: arial; font-size: 16; }}</style>");
                    mdText.AddRange(body);
                    mdText.Add(@"<style class=""fallback"">body{{visibility:hidden}}</style>");
                    mdText.Add(@"<script src =""https://casual-effects.com/markdeep/latest/markdeep.min.js"" charset=""utf-8""></script>");
                    mdText.Add(@"<script>window.alreadyProcessedMarkdeep||(document.body.style.visibility=""visible"")</script>");
                    break;

                case MarkdownMode.DarkApi:
                    mdText.Add(@"<meta charset=""utf-8"">");
                    mdText.Add(@"<link rel=""stylesheet"" href=""https://casual-effects.com/markdeep/latest/slate.css?"">");
                    mdText.AddRange(body);
                    mdText.Add(@"<style class=""fallback"">body{{visibility:hidden}}</style>");
                    mdText.Add(@"<script>markdeepOptions={tocStyle:'long'};</script>");
                    mdText.Add(@"<script src =""https://casual-effects.com/markdeep/latest/markdeep.min.js"" charset=""utf-8""></script>");
                    mdText.Add(@"<script>window.alreadyProcessedMarkdeep||(document.body.style.visibility=""visible"")</script>");
                    break;

                case MarkdownMode.LightApi:
                    mdText.Add(@"<meta charset=""utf-8"">");
                    mdText.Add(@"<link rel=""stylesheet"" href=""https://casual-effects.com/markdeep/latest/apidoc.css?"">");
                    mdText.AddRange(body);
                    mdText.Add(@"<style class=""fallback"">body{visibility:hidden}</style><script>markdeepOptions={tocStyle:'medium'};</script>");
                    mdText.Add(@"<script src=""https://casual-effects.com/markdeep/latest/markdeep.min.js?"" charset=""utf-8""></script>");
                    mdText.Add(@"<script>window.alreadyProcessedMarkdeep||(document.body.style.visibility=""visible"")</script>");
                    break;
            }

            string htmlText = string.Join(Environment.NewLine, mdText);

            if (show)
            {
                string fn = Path.GetTempFileName() + ".html";
                File.WriteAllText(fn, string.Join(Environment.NewLine, mdText));
                new Process { StartInfo = new ProcessStartInfo(fn) { UseShellExecute = true } }.Start();
            }

            return htmlText;
        }


    }
}
