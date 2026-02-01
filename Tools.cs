using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;


namespace Ephemera.NBagOfTricks
{
    /// <summary>
    /// Higher level tools.
    /// </summary>
    public static class Tools
    {
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
        public static string MarkdownToHtml(List<string> body, MarkdownMode mode, bool show)
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
                try
                {
                    File.WriteAllText(fn, string.Join(Environment.NewLine, mdText));
                    new Process { StartInfo = new ProcessStartInfo(fn) { UseShellExecute = true } }.Start();
                }
                finally
                {
                    File.Delete(fn);
                }
            }

            return htmlText;
        }

        /// <summary>
        /// Execute a chunk of lua code (not file).
        /// </summary>
        /// <param name="scode">The code</param>
        /// <returns>Exit code, stdout</returns>
        public static (int ecode, string sret) ExecuteLuaCode(string scode)
        {
            var fn = Path.GetTempFileName();
            var ecode = 0;
            var sret = "";

            try
            {
                File.WriteAllText(fn, scode);
                ProcessStartInfo pinfo = new("lua", [fn])
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                using Process proc = new() { StartInfo = pinfo };
                proc.Start();

                // TIL: To avoid deadlocks, always read the output stream first and then wait.
                var stdout = proc.StandardOutput.ReadToEnd();
                var stderr = proc.StandardError.ReadToEnd();

                proc.WaitForExit();

                ecode = proc.ExitCode == 0 ? 0 : proc.ExitCode;
                sret = proc.ExitCode == 0 ? stdout : stderr;
            }
            catch (Exception ex)
            {
                ecode = -1;
                sret = $"Failed: {ex}";
            }
            finally
            {
                File.Delete(fn);
            }

            return (ecode, sret);
        }

        /// <summary>
        /// Run a script in external process. Handles py/lua/ps1/cmd/bat. This is primarily to be used as a
        /// run-to-completion format. Currently there is no clean way to kill a running script or to support
        /// continuous input.
        /// Liberally borrowed from http://csharptest.net/532/using-processstart-to-capture-console-output/index.html
        /// </summary>
        /// <param name="fn">Script name</param>
        /// <param name="output">What to do with script stdout</param>
        /// <param name="error">What to do with script stderr</param>
        /// <param name="input">Optional for stdin - one-shot</param>
        /// <returns>Script return code</returns>
        public static int RunScript(string fn, Action<string> output, Action<string> error, TextReader? input = null)
        {
            int retCode = -1;
            var ext = Path.GetExtension(fn);
            var wdir = Path.GetDirectoryName(fn);

            string[] args = ext switch
            {
                ".cmd" or ".bat" => ["cmd", "/C", fn],
                ".ps1" => ["powershell", "-executionpolicy", "bypass", "-File", fn],
                ".lua" => ["lua", fn],
                ".py" => ["py", "-u", fn],  // TODO -u is unbuffered/autoflush - optional?
                _ => ["cmd", "/C", fn] // default just open.
            };

            ProcessStartInfo pinfo = new(args[0], args[1..])
            {
                UseShellExecute = false,
                RedirectStandardInput = input is not null,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                //ErrorDialog = false,
                WorkingDirectory = wdir,
            };

            // - The OutputDataReceived event fires on a separate thread, so you must use synchronization mechanisms
            //   (like ManualResetEvent or TaskCompletionSource) if you need to ensure all output is captured before
            //   proceeding with or using the full output in the main thread (e.g. after calling process.WaitForExit()).
            // - Also! in order to receive the events asynchronously, the target must flush its stdout on write.

            using (var proc = Process.Start(pinfo))
            using (ManualResetEvent mreOut = new(false), mreErr = new(false))
            {
                if (proc is null) { throw new InvalidOperationException($"Couldn't start process {args}"); }

                proc.OutputDataReceived += (o, e) =>
                {
                    if (e.Data == null) mreOut.Set();
                    else output(e.Data);
                };
                proc.BeginOutputReadLine();

                proc.ErrorDataReceived += (o, e) =>
                {
                    if (e.Data == null) mreErr.Set();
                    else error(e.Data);
                };
                proc.BeginErrorReadLine();

                // Optional input to script. Not usual.
                if (input is not null)
                {
                    bool done = false;
                    while (input != null && !done)
                    {
                        var line = input.ReadLine();
                        if (line is not null) { proc.StandardInput.WriteLine(line); }
                        else { done = true; }
                    }
                    proc.StandardInput.Close();
                }

                proc.WaitForExit();
                 
                mreOut.WaitOne();
                mreErr.WaitOne();

                retCode = proc.ExitCode;
            }

            return retCode;
        }

        /// <summary>
        /// Reports non-ascii characters in a file. UTF-8 only.
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static List<string> SniffBin(string fn, int limit = 100)
        {
            List<string> res = [];

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
                                if (res.Count >= limit)
                                {
                                    res.Add($"...... there are more ......");
                                    done = true;
                                }
                            }
                            col++;
                            break;
                    }
                    i++;
                }
            }

            return res;
        }
    }
}
