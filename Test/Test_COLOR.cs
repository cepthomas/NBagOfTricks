using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfTricks.PNUT;


namespace Ephemera.NBagOfTricks.Test
{
    public class COLOR_CONV : TestSuite
    {
        public override void RunSuite()
        {
            Info("Test color conversion functions.");

            //ColorDialog d = new();
            //d.ShowDialog();
            //return;

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
            "Now we can begin working on lots.",
            "Even the worst thing we can do here is good.",
            "Let's do it again then, what the heck.",
            "Everything's not great in life.",
            "Use what happens naturally, don't fight it.",
            "How do you make a round circle?",
            "That's your challenge for the day."
        ];

        static string FormatForHtml(Color clr)
        {
            return $"#{clr.R:x2}{clr.G:x2}{clr.B:x2}";
        }

        /// <summary>
        /// A dumper - not a unit test.
        /// </summary>
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
                var sysclr = conclr.ToSystemColor();
                var conclr_name = Enum.GetName(typeof(ConsoleColor), conclr);

                // --- Console ---
                Console.BackgroundColor = conclr;
                Console.WriteLine($"ConsoleColor:{conclr} System.Color:{sysclr.Name} Words:{_ross[i]}");
                Console.ResetColor();

                // --- html ---
                html.Add($"<span style=\"font-size: 20pt; color: black; background-color: white;\">ConsoleColor:{conclr} System.Color:{sysclr.Name}");
                html.Add($"<span style=\"color: black; background-color: {FormatForHtml(sysclr)};\">|  {_ross[i]} |");
                html.Add($"<br>");
            }

            html.Add("</body></html>");
            var fn = Path.Combine(MiscUtils.GetSourcePath(), "out", "ConsoleColorToSystem.html");
            File.WriteAllLines(fn, html);
        }

        /// <summary>
        /// A dumper - not a unit test.
        /// </summary>
        public void SystemColorToConsoleColor()
        {
            Console.WriteLine("");
            Console.WriteLine($"----- Convert System Color To ConsoleColor -----");
            //Console.WriteLine($"----- Dump to SystemColorToConsoleColor.html -----");

            // Colors that should roughly match ConsoleColors. Some(*) don't align well by name.
            List<Color> colorsMatch =
            [
                Color.Black,
                Color.DarkBlue,
                Color.DarkGreen,
                Color.DarkCyan,
                Color.DarkRed,
                Color.DarkMagenta,
                Color.Olive, // aka DarkYellow
                Color.LightGray, // Gray*
                Color.DimGray, // DarkGray*
                Color.Blue,
                Color.Lime, // Green* is very dark
                Color.Cyan,
                Color.Red,
                Color.Magenta,
                Color.Yellow,
                Color.White,
            ];

            // Explicit. Seems to work better.
            List<Color> colors =
            [
                // System.Color                      ConsoleColor  System.Color.Name
                ColorUtils.MakeColor(0x00, 0x00, 0x00), // 0000          Black
                ColorUtils.MakeColor(0x00, 0x00, 0x80), // 0001          Navy
                ColorUtils.MakeColor(0x00, 0x80, 0x00), // 0010          Green
                ColorUtils.MakeColor(0x00, 0x80, 0x80), // 0011          Teal
                ColorUtils.MakeColor(0x80, 0x00, 0x00), // 0100          Maroon
                ColorUtils.MakeColor(0x80, 0x00, 0x80), // 0101          Purple
                ColorUtils.MakeColor(0x80, 0x80, 0x00), // 0110          Olive
                ColorUtils.MakeColor(0xC0, 0xC0, 0xC0), // 0111          Silver
                ColorUtils.MakeColor(0x80, 0x80, 0x80), // 1000          Gray
                ColorUtils.MakeColor(0x00, 0x00, 0xFF), // 1001          Blue
                ColorUtils.MakeColor(0x00, 0xFF, 0x00), // 1010          Lime
                ColorUtils.MakeColor(0x00, 0xFF, 0xFF), // 1011          Aqua
                ColorUtils.MakeColor(0xFF, 0x00, 0x00), // 1100          Red
                ColorUtils.MakeColor(0xFF, 0x00, 0xFF), // 1101          Fuchsia
                ColorUtils.MakeColor(0xFF, 0xFF, 0x00), // 1110          Yellow
                ColorUtils.MakeColor(0xFF, 0xFF, 0xFF)  // 1111          White
            ];

            List<string> html = [];
            html.Add("<!DOCTYPE html><html><head><meta charset=\"utf-8\"></head><body>");
            html.Add($"<span style=\"font-size: 24pt; color: black; background-color: white;\">Convert System Color To ConsoleColor<br>");

            foreach (var clr in colors)
            {
                // var sysclr_name = clr.Name;
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
            Info("Test ANSI color functions.");

            //char ESC = 27;
            // One of: ESC[IDm  ESC[38;5;IDm  ESC[48;5;IDm  ESC[38;2;R;G;Bm  ESC[48;2;R;G;Bm

            var (fg, bg) = ColorUtils.ColorFromAnsi("bad string");
            Assert(fg.IsEmpty);
            Assert(bg.IsEmpty);

            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[34m");
            Assert(fg.Name == "ff00007f");
            Assert(bg.Name == "0");

            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[45m");
            Assert(fg.Name == "0");
            Assert(bg.Name == "ff7f007f");

            // system
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;5;12m");
            Assert(fg.Name == "ff0000ff");
            Assert(bg.Name == "0");

            // id
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;5;122m");
            Assert(fg.Name == "ff87ffd7");
            Assert(bg.Name == "0");

            // grey
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;5;249m");
            Assert(fg.Name == "ffb2b2b2");
            Assert(bg.Name == "0");

            // id bg
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[48;5;231m");
            Assert(fg.Name == "0");
            Assert(bg.Name == "ffffffff");

            //ESC[38;2;R;G;Bm
            // rgb
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[38;2;204;39;187m");
            Assert(fg.Name == "ffcc27bb");
            Assert(bg.Name == "0");

            // rgb invert
            (fg, bg) = ColorUtils.ColorFromAnsi("\u001b[48;2;19;0;222m");
            Assert(fg.Name == "0");
            Assert(bg.Name == "ff1300de");
        }
    }
}
