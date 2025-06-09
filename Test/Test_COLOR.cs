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
    public class COLOR_STUFF : TestSuite
    {
        public override void RunSuite()
        {
            //ColorDialog d = new();
            //d.ShowDialog();
            //return;

            UT_INFO("Tests color translation functions.");

            ConsoleColorToSystem();

            SystemColorToConsoleColor();
        }

        readonly List<string> _ross =
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
            "How do you make a round circle with a square knife?",
            "That's your challenge for the day."
        ];

        string FormatForHtml(Color clr)
        {
            return $"#{clr.R:x2}{clr.G:x2}{clr.B:x2}";
        }


        //================================================================================================
        public void ConsoleColorToSystem()
        {
            Console.WriteLine("");
            Console.WriteLine($"----- Convert ConsoleColor to System Color -----");
            //Console.WriteLine($"----- Dump to Console and ConsoleColorToSystem.html -----");

            List<string> html = [];
            html.Add("<!DOCTYPE html><html><head><meta charset=\"utf-8\"></head><body>");
            html.Add($"<span style=\"font-size: 24pt; color: black; background-color: white;\">Convert ConsoleColor to System Color<br>");

            var cvals = Enum.GetValues(typeof(ConsoleColor));

            for (int i = 0; i < cvals.Length; i++)
            {
                var conclr = (ConsoleColor)i;

                // --- Console ---
                //Console.ForegroundColor = conclr;
                //Console.BackgroundColor = (ConsoleColor)(cvals.Count - i - 1);
                Console.BackgroundColor = conclr;
                Console.WriteLine($"FG:{Console.ForegroundColor} BG:{Console.BackgroundColor} {_ross[i]}");
                Console.ResetColor();

                // --- html ---
                var sysclr = conclr.ToSystemColor();
                var name = Enum.GetName(typeof(ConsoleColor), conclr);
                html.Add($"<span style=\"font-size: 20pt; color: black; background-color: white;\">{name}");
                html.Add($"<span style=\"color: black; background-color: {FormatForHtml(sysclr)};\">|  {_ross[i]} |");
                html.Add($"<br>");
            }

            html.Add("</body></html>");
            var fn = Path.Combine(MiscUtils.GetSourcePath(), "out", "ConsoleColorToSystem.html");
            File.WriteAllLines(fn, html);
        }


        //================================================================================================
        public void SystemColorToConsoleColor()
        {
            Console.WriteLine("");
            Console.WriteLine($"----- Convert System Color To ConsoleColor -----");
            //Console.WriteLine($"----- Dump to SystemColorToConsoleColor.html -----");

            // Colors that should roughly match ConsoleColors. Some(*) don't align.
            List<Color> colors =
            [
                Color.Black,
                Color.White,
                Color.LightGray, // Gray*
                Color.DimGray, // DarkGray*
                Color.Red,
                Color.DarkRed,
                Color.Lime, // Green* is very dark
                Color.DarkGreen,
                Color.Blue,
                Color.DarkBlue,
                Color.Cyan,
                Color.DarkCyan,
                Color.Magenta,
                Color.DarkMagenta,
                Color.Yellow,
                Color.Olive, // aka DarkYellow
            ];

            List<string> html = [];
            html.Add("<!DOCTYPE html><html><head><meta charset=\"utf-8\"></head><body>");
            html.Add($"<span style=\"font-size: 24pt; color: black; background-color: white;\">Convert System Color To ConsoleColor<br>");
            //html.Add($"<span style=\"font-size: 16pt; color: black; background-color: white;\">TODO need to reconvert and compare<br>");

            foreach (var clr in colors)
            {
                var conclr = clr.ToConsoleColor();
                var conclr_name = Enum.GetName(typeof(ConsoleColor), conclr);

                // --- Console ---
                Console.Write($"System Color:{clr.Name} ");
                Console.BackgroundColor = conclr;
                Console.Write($"ConsoleColor:{conclr_name} ");
                Console.WriteLine("");
                Console.ResetColor();
                //Console.WriteLine($"{sysclr.Name} => {sysclr.R} {sysclr.G} {sysclr.B} => {conclr}");

                // --- html ---
                html.Add($"<span style=\"font-size: 16pt; color: black; background-color: white;\">{clr.Name}");
                html.Add($"<span style=\"color: black; background-color: {clr.Name};\">|  {_ross[0]}  |");
                html.Add($"<span style=\"color: white; background-color: {clr.Name};\">|  {_ross[1]}  |");
                html.Add($"<span style=\"color: {clr.Name}; background-color: black;\">|  {_ross[2]}  |");
                html.Add($"<span style=\"color: {clr.Name}; background-color: white;\">|  {_ross[3]}  |");
                html.Add($"<br>");
            }
            html.Add("</body></html>");

            var fn = Path.Combine(MiscUtils.GetSourcePath(), "out", "SystemColorToConsoleColor.html");
            File.WriteAllLines(fn, html);
        }
    }

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
}
