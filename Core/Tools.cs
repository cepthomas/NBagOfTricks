using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Drawing;


namespace Ephemera.NBagOfTricks
{
    /// <summary>
    /// Higher level than Utils.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Reports non-ascii characters in a file. UTF-8 only.
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static List<string> SniffBin(string fn, int limit = 100)
        {
            List<string> res = new();

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
                File.WriteAllText(fn, string.Join(Environment.NewLine, mdText));
                new Process { StartInfo = new ProcessStartInfo(fn) { UseShellExecute = true } }.Start();
            }

            return htmlText;
        }
    }
}
