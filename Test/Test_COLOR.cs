using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class COLOR_ANSI : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests ANSI color functions.");

            //char ESC = 27;
            // One of: ESC[IDm  ESC[38;5;IDm  ESC[48;5;IDm  ESC[38;2;R;G;Bm  ESC[48;2;R;G;Bm

            var (fg, bg) = ColorUtils.ColorFromAnsi("bad string");
            UT_TRUE(fg.IsEmpty);
            UT_TRUE(bg.IsEmpty);

            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[34m");
            UT_EQUAL(fg.Name, "ff00007f");
            UT_EQUAL(bg.Name, "0");

            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[45m");
            UT_EQUAL(fg.Name, "0");
            UT_EQUAL(bg.Name, "ff7f007f");

            // system
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;5;12m");
            UT_EQUAL(fg.Name, "ff0000ff");
            UT_EQUAL(bg.Name, "0");

            // id
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;5;122m");
            UT_EQUAL(fg.Name, "ff87ffd7");
            UT_EQUAL(bg.Name, "0");

            // grey
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;5;249m");
            UT_EQUAL(fg.Name, "ffb2b2b2");
            UT_EQUAL(bg.Name, "0");

            // id bg
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[48;5;231m");
            UT_EQUAL(fg.Name, "0");
            UT_EQUAL(bg.Name, "ffffffff");

            //ESC[38;2;R;G;Bm
            // rgb
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;2;204;39;187m");
            UT_EQUAL(fg.Name, "ffcc27bb");
            UT_EQUAL(bg.Name, "0");

            // rgb invert
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[48;2;19;0;222m");
            UT_EQUAL(fg.Name, "0");
            UT_EQUAL(bg.Name, "ff1300de");
        }
    }

    public class COLOR_STUFF : TestSuite
    {
        public override void RunSuite()
        {
            UT_INFO("Tests color translation functions.");

            //================================================================================================
            {
                Console.WriteLine("");
                Console.WriteLine($"===== DumpConsoleColors ======");

                List<string> ross =
                [
                    "You have freedom here.",
                "The only guide is your heart.",
                "We can always carry this a step further.",
                "There's really no end to this.",
                "Let's give him a friend too.",
                "Everybody needs a friend.",
                "Follow the lay of the land.",
                "It's most important.",
                "Only eight colors that you need.",
                "Now we can begin working on lots of happy little things.",
                "Even the worst thing we can do here is good.",
                "Let's do it again then, what the heck.",
                "Everything's not great in life, but we can still find beauty in it.",
                "Use what happens naturally, don't fight it.",
                "How do you make a round circle with a square knife? ",
                "That's your challenge for the day."
                ];

                var cvals = Enum.GetValues(typeof(ConsoleColor));


                //var cvals = Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>().ToList();

                for (int i = 0; i < cvals.Length; i++)
                {
                    //Console.ForegroundColor = (ConsoleColor)i;
                    Console.BackgroundColor = (ConsoleColor)i;
                    //Console.BackgroundColor = (ConsoleColor)(cvals.Count - i - 1);
                    Console.WriteLine($"FG:{Console.ForegroundColor} BG:{Console.BackgroundColor} {ross[i]}");
                    Console.ResetColor();
                }
            }


            //================================================================================================
            {
                Console.WriteLine("");
                Console.WriteLine($"===== ConvertToConsoleColor ======");

                List<Color> colors =
                [
                    Color.Black,
                    Color.DarkBlue,
                    Color.DarkGreen,
                    Color.DarkCyan,
                    Color.DarkRed,
                    Color.DarkMagenta,
                    Color.Gold, // was DarkYellow, TODO not right
                    Color.Gray,
                    Color.DarkGray,
                    Color.Blue,
                    Color.Green,
                    Color.Cyan,
                    Color.Red,
                    Color.Magenta,
                    Color.Yellow,
                    Color.White
                ];

                List<string> html = [];
                html.Add("<!DOCTYPE html><html><head><meta charset=\"utf-8\"></head><body>");
                foreach (var clr in colors)
                {
                    var conclr1 = ColorUtils.ConvertToConsoleColor(clr);
                    var conclr2 = ColorUtils.ConvertToConsoleColor(clr);
                    //html.Add($"<span style=\"font-size: 2.0em; background-color: #{clr.ToArgb()}; color: #ffffff; \">System:{clr.Name}</span><span style=\"font-size: 2.0em; background-color: #{conclr.}; color: #ffffff; \">Console:{conclr}</span><br>");
                    html.Add($"<span style=\"font-size: 2.0em; background-color: {clr.Name}; color: #ffffff; \">System:{clr.Name}   Console1:{conclr1}   Console2:{conclr2}</span><br>");
                }
                html.Add("</body></html>");

                File.WriteAllLines(@"C:\Dev\Treex\ConvertToConsoleColor.html", html);
            }


            //================================================================================================
            {
                Console.WriteLine("");
                Console.WriteLine($"===== DumpConsoleColorsToHtml ======");

                List<string> html = [];
                html.Add("<!DOCTYPE html><html><head><meta charset=\"utf-8\"></head><body>");
                var cvals = Enum.GetValues(typeof(ConsoleColor));
                for (int i = 0; i < cvals.Length; i++)
                {

                    var cc = ColorUtils.ConvertToSystemColor((ConsoleColor)i);
                    var name = Enum.GetName(typeof(ConsoleColor), i);
                    html.Add($"<span style=\"font-size: 20pt; background-color: #ffffff; color: #000000;\">{cc.Name} |");
                    html.Add($"<span style=\"background-color: #{cc.ToArgb():x6}; color: #ffffff;\">{name} |");

                    html.Add($"<br>");


                    //html.Add($"<span style=\"font-size: 2.0em; background-color: #{cc.ActualRGB:0x}; color: #ffffff; >ActualRGB:{cc.Name}  background-color: #{cc.Actual2RGB:0x}; color: #ffffff; >Actual2RGB:{cc.Name}   background-color: #{cc.ScreenShotSample:0x}; color: #ffffff; \">ScreenShotSample:{cc.Name}   </span><br>");
                }
                html.Add("</body></html>");
                File.WriteAllLines(@"C:\Dev\Treex\DumpConsoleColorsToHtml.html", html);
            }
        }
    }
}
