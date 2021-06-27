using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Diagnostics;


namespace NBagOfTricks
{
    /// <summary>
    /// Higher level than Utils.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Rudimentary C# source code formatter to make generated files somewhat readable.
        /// TODO handle/clean ugly braces.
        /// </summary>
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
        /// Reports non-ascii characters in a file. UTF-8 only.
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static List<string> SniffBin(string fn, int limit = 100)
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
        /// <returns></returns>
        public static string MarkdownToHtml(List<string> body, string bgcolor, string font, bool show)
        {
            // Put it together.
            var htmlText = new List<string>()
            {
                // Boilerplate
                $"<!DOCTYPE html><html><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">",
                $"<style>body {{ background-color: {bgcolor}; font-family: {font}; }}",
                $"</style></head><body>"
            };

            // Meat.
            htmlText.AddRange(body);

            // Bottom.
            htmlText.Add($"<!-- Markdeep: --><style class=\"fallback\">body{{visibility:hidden;white-space:pre;font-family:{font}}}</style><script src=\"markdeep.min.js\" charset=\"utf-8\"></script><script src=\"https://casual-effects.com/markdeep/latest/markdeep.min.js\" charset=\"utf-8\"></script><script>window.alreadyProcessedMarkdeep||(document.body.style.visibility=\"visible\")</script>");
            htmlText.Add($"</body></html>");
            string sret = string.Join(Environment.NewLine, htmlText);

            if (show)
            {
                string fn = Path.GetTempFileName() + ".html";
                File.WriteAllText(fn, string.Join(Environment.NewLine, htmlText));
                new Process { StartInfo = new ProcessStartInfo(fn) { UseShellExecute = true } }.Start();
            }

            return sret;
        }
    }
}
