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
        static Dictionary<(int, int, int), KnownColor>? _colorLUT = null;

        /// <summary>
        /// Decode ansi escape sequence arguments. TODO AnsiFromColor()
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
                    throw new ArgumentException($"ansi:[{ArgumentException}]");
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

        /// <summary>
        /// Creates a Color object from rgb. If it is in the named collection, the name is applied.
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public static Color MakeColor(int red, int green, int blue)
        {
            Color res;

            if (_colorLUT == null)
            {
                _colorLUT = new();

                // Init the cache.
                for (KnownColor iterclr = KnownColor.AliceBlue; iterclr <= KnownColor.YellowGreen; iterclr++)
                {
                    Color clr = Color.FromKnownColor(iterclr);
                    // Note that there are a couple of rgb values that have more than one name: Aqua-Cyan  Fuchsia-Magenta
                    _colorLUT[(clr.R, clr.G, clr.B)] = iterclr;
                }
            }

            // Do the real work.
            if (_colorLUT.TryGetValue((red, green, blue), out KnownColor knclr))
            {
                res = Color.FromKnownColor(knclr);
            }
            else
            {
                res = Color.FromArgb(red, green, blue);
            }

            return res;
        }

        /// <summary>Convert ConsoleColor to System.Color.</summary>
        /// <param name="conclr">In color</param>
        /// <returns>Out color</returns>
        public static Color ToSystemColor(this ConsoleColor conclr)
        {
            // System.Color names don't match ConsoleColor very well. Let's just bin them arbitrarily.

            var ret = conclr switch
            {
                // ConsoleColor.Name        System.Color                    ConsoleColor val  System.Color.Name
                ConsoleColor.Black       => MakeColor(0x00, 0x00, 0x00), // 0000              Black
                ConsoleColor.DarkBlue    => MakeColor(0x00, 0x00, 0x80), // 0001              Navy
                ConsoleColor.DarkGreen   => MakeColor(0x00, 0x80, 0x00), // 0010              Green
                ConsoleColor.DarkCyan    => MakeColor(0x00, 0x80, 0x80), // 0011              Teal
                ConsoleColor.DarkRed     => MakeColor(0x80, 0x00, 0x00), // 0100              Maroon
                ConsoleColor.DarkMagenta => MakeColor(0x80, 0x00, 0x80), // 0101              Purple
                ConsoleColor.DarkYellow  => MakeColor(0x80, 0x80, 0x00), // 0110              Olive
                ConsoleColor.Gray        => MakeColor(0xC0, 0xC0, 0xC0), // 0111              Silver 
                ConsoleColor.DarkGray    => MakeColor(0x80, 0x80, 0x80), // 1000              Gray
                ConsoleColor.Blue        => MakeColor(0x00, 0x00, 0xFF), // 1001              Blue
                ConsoleColor.Green       => MakeColor(0x00, 0xFF, 0x00), // 1010              Lime
                ConsoleColor.Cyan        => MakeColor(0x00, 0xFF, 0xFF), // 1011              Aqua
                ConsoleColor.Red         => MakeColor(0xFF, 0x00, 0x00), // 1100              Red
                ConsoleColor.Magenta     => MakeColor(0xFF, 0x00, 0xFF), // 1101              Fuchsia
                ConsoleColor.Yellow      => MakeColor(0xFF, 0xFF, 0x00), // 1110              Yellow
                ConsoleColor.White       => MakeColor(0xFF, 0xFF, 0xFF), // 1111              White
                _ => throw new ArgumentException(nameof(conclr)),
            };
            return ret;
        }

        /// <summary>Convert System.Color to ConsoleColor.</summary>
        /// <param name="sysclr">The color</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ConsoleColor ToConsoleColor(this Color sysclr)
        {
            ConsoleColor conclr;
            var sat = sysclr.GetSaturation(); // 0.0 => 1.0
            var brt = sysclr.GetBrightness(); // 0.0 => 1.0
            var hue = (int)Math.Round(sysclr.GetHue() / 60, MidpointRounding.AwayFromZero); // 0 => 5
            var grayish = sat < 0.5;
            var darkish = brt < 0.5;

            if (grayish)
            {
                conclr = (int)(brt * 3.5) switch
                {
                    0 => ConsoleColor.Black,
                    1 => ConsoleColor.DarkGray,
                    2 => ConsoleColor.Gray,
                    _ => ConsoleColor.White,
                };
            }
            else // color
            {
                conclr = hue switch
                {
                    1 => darkish ? ConsoleColor.DarkYellow : ConsoleColor.Yellow,
                    2 => darkish ? ConsoleColor.DarkGreen : ConsoleColor.Green,
                    3 => darkish ? ConsoleColor.DarkCyan : ConsoleColor.Cyan,
                    4 => darkish ? ConsoleColor.DarkBlue : ConsoleColor.Blue,
                    5 => darkish ? ConsoleColor.DarkMagenta : ConsoleColor.Magenta,
                    _ => darkish ? ConsoleColor.DarkRed : ConsoleColor.Red
                };
            }

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


        /// <summary>
        /// Helper to get next contrast color in the sequence.
        /// From http://colorbrewer2.org qualitative.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="dark">Dark or light series, usually dark.</param>
        /// <returns></returns>
        public static Color GetSequenceColor(int i, bool dark = true)
        {
            Color col = Color.Black;

            switch (i % 8)
            {
                case 0: col = dark ? Color.FromArgb(27, 158, 119) : Color.FromArgb(141, 211, 199); break;
                case 1: col = dark ? Color.FromArgb(217, 95, 2) : Color.FromArgb(255, 255, 179); break;
                case 2: col = dark ? Color.FromArgb(117, 112, 179) : Color.FromArgb(190, 186, 218); break;
                case 3: col = dark ? Color.FromArgb(231, 41, 138) : Color.FromArgb(251, 128, 114); break;
                case 4: col = dark ? Color.FromArgb(102, 166, 30) : Color.FromArgb(128, 177, 211); break;
                case 5: col = dark ? Color.FromArgb(230, 171, 2) : Color.FromArgb(253, 180, 98); break;
                case 6: col = dark ? Color.FromArgb(166, 118, 29) : Color.FromArgb(179, 222, 105); break;
                case 7: col = dark ? Color.FromArgb(102, 102, 102) : Color.FromArgb(252, 205, 229); break;
            }

            return col;
        }

        /// <summary>
        /// Mix two colors.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static Color HalfMix(Color one, Color two)
        {
            return Color.FromArgb(
                (one.A + two.A) >> 1,
                (one.R + two.R) >> 1,
                (one.G + two.G) >> 1,
                (one.B + two.B) >> 1);
        }

    }
}
