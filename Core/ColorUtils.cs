using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Linq;


namespace Ephemera.NBagOfTricks
{
    /// <summary>ConsoleColor variation with None - can be sited in UI controls.</summary>
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


        #region Types
        //https://stackoverflow.com/questions/1988833/converting-color-to-consolecolor
        readonly record struct ConsoleColorInfo
        (
            /// <summary>In ConsoleColor enum</summary>
            int Index,
            /// <summary>From ConsoleColor enum</summary>
            string Name,
            /// <summary>See ref</summary>
            uint EgaBRGB,
            /// <summary>From code maybe</summary>
            uint ActualRGB,
            /// <summary>From ??</summary>
            uint Actual2RGB,
            /// <summary>Color.Name vaue</summary>
            uint SysColorOfName,
            /// <summary>Screen sample</summary>
            uint ScreenShotSample,
            /// <summary></summary>
            uint MySet
        );
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

        // TODO clean up
        // Use Actual2RGB except DarkYellow which is closer to ScreenShotSample
        // For that, use #BA8E23. Often described as similar to mustard or ochre.
        // Other shades of dark yellow can be represented by hex codes like:
        // #9B870C  #8B8000  #FFA600  #D7C32A  0XFFD700-gold
        readonly static ConsoleColorInfo[] _consColorsAll =
        [
            //                          EGA      Actual    Actual2   SysColor  ScrShot   MySet
            new(    0, "Black",         0x0000,  0x000000, 0x000000, 0x000000, 0x000000, 0x000000),
            new(    1, "DarkBlue",      0x0001,  0x000080, 0x000080, 0x00008B, 0x3465A4, 0x000080),
            new(    2, "DarkGreen",     0x0010,  0x008000, 0x008000, 0x006400, 0x4E9A06, 0x008000),
            new(    3, "DarkCyan",      0x0011,  0x008080, 0x008080, 0x008B8B, 0x06989A, 0x008080),
            new(    4, "DarkRed",       0x0100,  0x800000, 0x800000, 0x8B0000, 0xCC0000, 0x800000),
            new(    5, "DarkMagenta",   0x0101,  0x012456, 0x800080, 0x8B008B, 0x75507B, 0x800080),
            new(    6, "DarkYellow",    0x0110,  0xEEEDF0, 0x808000, 0x000000, 0xC4A000, 0xBA8E23),
            new(    7, "Gray",          0x0111,  0xC0C0C0, 0xC0C0C0, 0x808080, 0xD3D7CF, 0xC0C0C0),
            new(    8, "DarkGray",      0x1000,  0x808080, 0x808080, 0xA9A9A9, 0x555753, 0x808080),
            new(    9, "Blue",          0x1001,  0x0000FF, 0x0000FF, 0x0000FF, 0x729FCF, 0x0000FF),
            new(   10, "Green",         0x1010,  0x00FF00, 0x00FF00, 0x008000, 0x8AE234, 0x00FF00),
            new(   11, "Cyan",          0x1011,  0x00FFFF, 0x00FFFF, 0x00FFFF, 0x34E2E2, 0x00FFFF),
            new(   12, "Red",           0x1100,  0xFF0000, 0xFF0000, 0xFF0000, 0xEF2929, 0xFF0000),
            new(   13, "Magenta",       0x1101,  0xFF00FF, 0xFF00FF, 0xFF00FF, 0xAD7FA8, 0xFF00FF),
            new(   14, "Yellow",        0x1110,  0xFFFF00, 0xFFFF00, 0xFFFF00, 0xFCE94F, 0xFFFF00),
            new(   15, "White",         0x1111,  0xFFFFFF, 0xFFFFFF, 0xFFFFFF, 0xEEEEEC, 0xFFFFFF),
        ];
        //var cc = _consColors[i];
        //html.Add($"<span style=\"font-size: 20pt; background-color: #ffffff; color: #000000;\">{cc.Name} |");
        //html.Add($"<span style=\"background-color: #{cc.ActualRGB:x6}; color: #ffffff;\">ActualRGB |");
        //html.Add($"<span style=\"background-color: #{cc.Actual2RGB:x6}; color: #ffffff;\">Actual2RGB |");
        //html.Add($"<span style=\"background-color: #{cc.ScreenShotSample:x6}; color: #ffffff;\">ScreenShotSample |");
        //html.Add($"<span style=\"background-color: #{cc.MySet:x6}; color: #ffffff;\">MySet |");


        readonly static List<int> _consColors =
        [
            0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0xBA8E23, 0xC0C0C0,
            0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF
        ];

        public static Color ConvertToSystemColor(ConsoleColor conclr) // TODO broken.
        {
            return Color.FromArgb(_consColors[(int)conclr]);
        }


        public static ConsoleColor ConvertToConsoleColor(Color clr)
        {
            return GetConsoleColor1(clr);


         //   return GetConsoleColor2(clr);

        }

        ////////////////////////// stolen ///////////////////////////////////////
        static ConsoleColor GetConsoleColor1(Color color)
        {
            if (color.GetSaturation() < 0.5)
            {
                // we have a grayish color
                switch ((int)(color.GetBrightness() * 3.5))
                {
                    case 0: return ConsoleColor.Black;
                    case 1: return ConsoleColor.DarkGray;
                    case 2: return ConsoleColor.Gray;
                    default: return ConsoleColor.White;
                }
            }

            int hue = (int)Math.Round(color.GetHue() / 60, MidpointRounding.AwayFromZero);
            if (color.GetBrightness() < 0.4)
            {
                // dark color
                switch (hue)
                {
                    case 1: return ConsoleColor.DarkYellow;
                    case 2: return ConsoleColor.DarkGreen;
                    case 3: return ConsoleColor.DarkCyan;
                    case 4: return ConsoleColor.DarkBlue;
                    case 5: return ConsoleColor.DarkMagenta;
                    default: return ConsoleColor.DarkRed;
                }
            }

            // bright color
            switch (hue)
            {
                case 1: return ConsoleColor.Yellow;
                case 2: return ConsoleColor.Green;
                case 3: return ConsoleColor.Cyan;
                case 4: return ConsoleColor.Blue;
                case 5: return ConsoleColor.Magenta;
                default: return ConsoleColor.Red;
            }
        }


        ////////////////////////// stolen ///////////////////////////////////////
        static ConsoleColor GetConsoleColor2(Color c)
        //public ConsoleColor FromColor(Color c)
        {
            int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0; // Bright bit
            index |= (c.R > 64) ? 4 : 0; // Red bit
            index |= (c.G > 64) ? 2 : 0; // Green bit
            index |= (c.B > 64) ? 1 : 0; // Blue bit
            return (ConsoleColor)index;
        }

        ////////////////////////// stolen ///////////////////////////////////////
        // Public Shared Function ColorToConsoleColor(cColor As Color) As ConsoleColor
        //         Dim cc As ConsoleColor
        //         If Not System.Enum.TryParse(Of ConsoleColor)(cColor.Name, cc) Then
        //             Dim intensity = If(Color.Gray.GetBrightness() < cColor.GetBrightness(), 8, 0)
        //             Dim r = If(cColor.R >= &H80, 4, 0)
        //             Dim g = If(cColor.G >= &H80, 2, 0)
        //             Dim b = If(cColor.B >= &H80, 1, 0)

        //             cc = CType(intensity + r + g + b, ConsoleColor)
        //         End If
        //         Return cc
        //     End Function

    }
}
