using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Linq;


namespace Ephemera.NBagOfTricks
{
    #region Types
    /// <summary>ConsoleColor variation with None added.</summary>
    public enum ConsoleColorEx
    {
        None = -1,
        Black = ConsoleColor.Black,
        DarkBlue = ConsoleColor.DarkBlue,
        DarkGreen = ConsoleColor.DarkGreen,
        DarkCyan = ConsoleColor.DarkCyan,
        DarkRed = ConsoleColor.DarkRed,
        DarkMagenta = ConsoleColor.DarkMagenta,
        DarkYellow = ConsoleColor.DarkYellow,
        Gray = ConsoleColor.Gray,
        DarkGray = ConsoleColor.DarkGray,
        Blue = ConsoleColor.Blue,
        Green = ConsoleColor.Green,
        Cyan = ConsoleColor.Cyan,
        Red = ConsoleColor.Red,
        Magenta = ConsoleColor.Magenta,
        Yellow = ConsoleColor.Yellow,
        White = ConsoleColor.White
    }
    #endregion



    public static class ColorUtils
    {
        /// <summary>
        /// Decode ansi escape sequence arguments. TODOF AnsiFromColor()
        /// </summary>
        /// <param name="ansi">Ansi args string</param>
        /// <returns>Foreground and background colors. Color is Empty if invalid ansi string.</returns>
        public static (Color fg, Color bg) ColorFromAnsi(string ansi)
        {
            Color fg = Color.Empty;
            Color bg = Color.Empty;

            try
            {
                // Sanity check. ESC[34m  ESC[38;5;IDm  ESC[38;2;R;G;Bm
                if (ansi[0] != '\u001b' || ansi[1] != '[' || ansi.Last() != 'm')
                {
                    throw new Exception();
                }

                var parts = ansi[2..^1].SplitByToken(";").Select(i => int.Parse(i)).ToList();

                var p0 = parts.Count >= 1 ? parts[0] : 0;
                var p1 = parts.Count >= 2 ? parts[1] : 0;
                var p2 = parts.Count == 3 ? parts[2] : 0;

                switch (parts.Count)
                {
                    /////// Standard 8/16 colors. ESC[IDm
                    case 1 when p0 >= 30 && p0 <= 37:
                        fg = MakeStdColor(p0 - 30);
                        break;

                    case 1 when p0 >= 40 && p0 <= 47:
                        bg = MakeStdColor(p0 - 40);
                        //invert = true;
                        break;

                    case 1 when p0 >= 90 && p0 <= 97:
                        fg = MakeStdColor(p0 - 90);
                        break;

                    case 1 when p0 >= 100 && p0 <= 107:
                        bg = MakeStdColor(p0 - 100);
                        //invert = true;
                        break;

                    /////// 256 colors. ESC[38;5;IDm  ESC[48;5;IDm
                    case 3 when (p0 == 38 || p0 == 48) && p1 == 5 && p2 >= 0 && p2 <= 15:
                        // 256 colors - standard color.
                        var clr1 = MakeStdColor(p2);
                        if (p0 == 48) bg = clr1; else fg = clr1;
                        break;

                    case 3 when (p0 == 38 || p0 == 48) && p1 == 5 && p2 >= 16 && p2 <= 231:
                        // 256 colors - rgb color.
                        int[] map6 = [0, 95, 135, 175, 215, 255];
                        int im = p2 - 16;
                        int r = map6[im / 36];
                        int g = map6[(im / 6) % 6];
                        int b = map6[im % 6];

                        var clr2 = Color.FromArgb(r, g, b);
                        if (p0 == 48) bg = clr2; else fg = clr2;
                        break;

                    case 3 when (p0 == 38 || p0 == 48) && p1 == 5 && p2 >= 232 && p2 <= 255:
                        // 256 colors - grey
                        int i = p2 - 232; // 0 - 23
                        int grey = i * 10 + 8;

                        var clr3 = Color.FromArgb(grey, grey, grey);
                        if (p0 == 48) bg = clr3; else fg = clr3;
                        break;

                    /////// Explicit rgb colors. ESC[38;2;R;G;Bm  ESC[48;2;R;G;Bm
                    case 5 when p0 == 38 || p0 == 48 && p1 == 2:

                        var clr4 = Color.FromArgb(parts[2], parts[3], parts[4]);
                        if (p0 == 48) bg = clr4; else fg = clr4;
                        break;
                }

                static Color MakeStdColor(int id)
                {
                    (int r, int g, int b)[] std_colors = [
                    (0, 0, 0),       (127, 0, 0),   (0, 127, 0),   (127, 127, 0),
                    (0, 0, 127),     (127, 0, 127), (0, 127, 127), (191, 191, 191),
                    (127, 127, 127), (0, 0, 0),     (0, 255, 0),   (255, 255, 0),
                    (0, 0, 255),     (255, 0, 255), (0, 255, 255), (255, 255, 255)];
                    return Color.FromArgb(std_colors[id].r, std_colors[id].g, std_colors[id].b);
                }
            }
            catch (Exception)
            {
                fg = Color.Empty;
                bg = Color.Empty;
            }

            return (fg, bg);
        }

        /// <summary>Lookup table.</summary>
        readonly static Dictionary<ConsoleColor, Color> _conColors = new()
        {
            [ConsoleColor.Black]       = Color.FromArgb(000, 000, 000), // 0000
            [ConsoleColor.White]       = Color.FromArgb(255, 255, 255), // 1111
            [ConsoleColor.Gray]        = Color.FromArgb(192, 192, 192), // 0111  aka DarkWhite
            [ConsoleColor.DarkGray]    = Color.FromArgb(128, 128, 128), // 1000
            [ConsoleColor.Red]         = Color.FromArgb(255, 000, 000), // 1100
            [ConsoleColor.DarkRed]     = Color.FromArgb(128, 000, 000), // 0100
            [ConsoleColor.Green]       = Color.FromArgb(000, 255, 000), // 1010
            [ConsoleColor.DarkGreen]   = Color.FromArgb(000, 128, 000), // 0010
            [ConsoleColor.Blue]        = Color.FromArgb(000, 000, 255), // 1001
            [ConsoleColor.DarkBlue]    = Color.FromArgb(000, 000, 128), // 0001
            [ConsoleColor.Cyan]        = Color.FromArgb(000, 255, 255), // 1011
            [ConsoleColor.DarkCyan]    = Color.FromArgb(000, 128, 128), // 0011
            [ConsoleColor.Magenta]     = Color.FromArgb(255, 000, 255), // 1101
            [ConsoleColor.DarkMagenta] = Color.FromArgb(128, 000, 128), // 0101
            [ConsoleColor.Yellow]      = Color.FromArgb(255, 255, 000), // 1110
            [ConsoleColor.DarkYellow]  = Color.FromArgb(128, 128, 000), // 0110
        };

        /// <summary>Convert ConsoleColor to System Color.</summary>
        /// <param name="conclr">The color</param>
        /// <returns></returns>
        public static Color ToSystemColor(this ConsoleColor conclr)
        {
            return _conColors[conclr];
        }

        /// <summary>Convert System Color to ConsoleColor.</summary>
        /// <param name="sysclr">The color</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ConsoleColor ToConsoleColor(this Color sysclr)
        {
            ConsoleColor conclr;
            var sat = sysclr.GetSaturation();
            var brt = sysclr.GetBrightness();
            var hue = (int)Math.Round(sysclr.GetHue() / 60, MidpointRounding.AwayFromZero);

            if (sat < 0.5) // grayish
            {
                switch ((int)(brt * 3.5))
                {
                    case 0:  conclr = ConsoleColor.Black; break;
                    case 1:  conclr = ConsoleColor.DarkGray; break;
                    case 2:  conclr = ConsoleColor.Gray; break;
                    default: conclr = ConsoleColor.White; break;
                }
            }
            else if (brt < 0.4) // dark
            {
                switch (hue)
                {
                    case 1:  conclr = ConsoleColor.DarkYellow; break;
                    case 2:  conclr = ConsoleColor.DarkGreen; break;
                    case 3:  conclr = ConsoleColor.DarkCyan; break;
                    case 4:  conclr = ConsoleColor.DarkBlue; break;
                    case 5:  conclr = ConsoleColor.DarkMagenta; break;
                    default: conclr = ConsoleColor.DarkRed; break;
                }
            }

            else // normal
            {
                switch (hue)
                {
                    case 1:  conclr = ConsoleColor.Yellow; break;
                    case 2:  conclr = ConsoleColor.Green; break;
                    case 3:  conclr = ConsoleColor.Cyan; break;
                    case 4:  conclr = ConsoleColor.Blue; break;
                    case 5:  conclr = ConsoleColor.Magenta; break;
                    default: conclr = ConsoleColor.Red; break;
                }
            }

            //Console.WriteLine($"{sysclr.Name} => {sysclr.R} {sysclr.G} {sysclr.B} => {conclr}");

            return conclr;
        }

        /// <summary>Simple binning approach.</summary>
        /// <param name="sysclr"></param>
        /// <returns></returns>
        public static ConsoleColor ToConsoleColor_simple(this Color sysclr)
        {
            int val = (sysclr.R > 0x80 || sysclr.G > 0x80 || sysclr.B > 0x80) ? 8 : 0;
            int lim = 0x40;
            val |= sysclr.R > lim ? 4 : 0;
            val |= sysclr.G > lim ? 2 : 0;
            val |= sysclr.B > lim ? 1 : 0;
            return (ConsoleColor)val;
        }
    }
}
