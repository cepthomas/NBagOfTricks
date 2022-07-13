using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace NBagOfTricks
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

        /// <summary>
        /// Convert list of markdown lines to html.
        /// </summary>
        /// <param name="body">The md text.</param>
        /// <param name="bgcolor">Background color for page.</param>
        /// <param name="font">Main font-family.</param>
        /// <param name="show">If true open in browser.</param>
        /// <returns>Valid html.</returns>
        public static string MarkdownToHtml(List<string> body, Color bgcolor, Font font, bool show)
        {
            // Put it together.
            var mdText = new List<string>()
            {
                $"<style>body {{ background-color: {bgcolor.Name}; font-family: {font.Name}; font-size: {font.Size}; }}</style>"
            };

            // Meat.
            mdText.AddRange(body);

            //mdText.Add(@"
            //<style class=""fallback"">body{{visibility:hidden;white-space:pre;font-family:{font}}}</style>
            //<script>markdeepOptions={tocStyle:'long'};</script>
            //<script src =""https://casual-effects.com/markdeep/latest/markdeep.min.js"" charset=""utf-8""></script>
            //<script>window.alreadyProcessedMarkdeep||(document.body.style.visibility=""visible"")</script>;");

            mdText.Add(@"
            <style class=""fallback"">body{{visibility:hidden}}</style>
            <script src =""https://casual-effects.com/markdeep/latest/markdeep.min.js"" charset=""utf-8""></script>
            <script>window.alreadyProcessedMarkdeep||(document.body.style.visibility=""visible"")</script>;");
            
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
