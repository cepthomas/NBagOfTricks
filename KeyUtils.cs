using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Ephemera.NBagOfTricks
{
    /// <summary>
    /// Static keyboard processing functions.
    /// </summary>
    public static class KeyUtils
    {
        /* TODO used?
        /// <summary>Api version of win32.</summary>
        public enum KeyState
        {
            /// <summary>Nothing going on.</summary>
            NotPressed,
            /// <summary>Transient press.</summary>
            Pressed,
            /// <summary>e.g. caps lock.</summary>
            Toggled
        }

        /// <summary>General purpose decoder for keys. Because windows makes it kind of difficult.</summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <returns>Tuple of Converted char (0 if not convertible) and keyCode(s).</returns>
        public static (char ch, List<Keys> keyCodes) KeyToChar(Keys key, Keys modifiers)
        {
            char ch = (char)0;
            List<Keys> keyCodes = new();

            bool shift = modifiers.HasFlag(Keys.Shift);
            bool iscap = (Console.CapsLock && !shift) || (!Console.CapsLock && shift);

            // Check modifiers.
            if (modifiers.HasFlag(Keys.Control)) keyCodes.Add(Keys.Control);
            if (modifiers.HasFlag(Keys.Alt)) keyCodes.Add(Keys.Alt);
            if (modifiers.HasFlag(Keys.Shift)) keyCodes.Add(Keys.Shift);

            switch (key)
            {
                case Keys.Enter: ch = '\n'; break;
                case Keys.Tab: ch = '\t'; break;
                case Keys.Space: ch = ' '; break;
                case Keys.Back: ch = (char)8; break;
                case Keys.Escape: ch = (char)27; break;
                case Keys.Delete: ch = (char)127; break;

                case Keys.Left: keyCodes.Add(Keys.Left); break;
                case Keys.Right: keyCodes.Add(Keys.Right); break;
                case Keys.Up: keyCodes.Add(Keys.Up); break;
                case Keys.Down: keyCodes.Add(Keys.Down); break;

                case Keys.D0: ch = shift ? ')' : '0'; break;
                case Keys.D1: ch = shift ? '!' : '1'; break;
                case Keys.D2: ch = shift ? '@' : '2'; break;
                case Keys.D3: ch = shift ? '#' : '3'; break;
                case Keys.D4: ch = shift ? '$' : '4'; break;
                case Keys.D5: ch = shift ? '%' : '5'; break;
                case Keys.D6: ch = shift ? '^' : '6'; break;
                case Keys.D7: ch = shift ? '&' : '7'; break;
                case Keys.D8: ch = shift ? '*' : '8'; break;
                case Keys.D9: ch = shift ? '(' : '9'; break;

                case Keys.Oemplus: ch = shift ? '+' : '='; break;
                case Keys.OemMinus: ch = shift ? '_' : '-'; break;
                case Keys.OemQuestion: ch = shift ? '?' : '/'; break;
                case Keys.Oemcomma: ch = shift ? '<' : ','; break;
                case Keys.OemPeriod: ch = shift ? '>' : '.'; break;
                case Keys.OemQuotes: ch = shift ? '\"' : '\''; break;
                case Keys.OemSemicolon: ch = shift ? ':' : ';'; break;
                case Keys.OemPipe: ch = shift ? '|' : '\\'; break;
                case Keys.OemCloseBrackets: ch = shift ? '}' : ']'; break;
                case Keys.OemOpenBrackets: ch = shift ? '{' : '['; break;
                case Keys.Oemtilde: ch = shift ? '~' : '`'; break;

                case Keys.NumPad0: ch = '0'; break;
                case Keys.NumPad1: ch = '1'; break;
                case Keys.NumPad2: ch = '2'; break;
                case Keys.NumPad3: ch = '3'; break;
                case Keys.NumPad4: ch = '4'; break;
                case Keys.NumPad5: ch = '5'; break;
                case Keys.NumPad6: ch = '6'; break;
                case Keys.NumPad7: ch = '7'; break;
                case Keys.NumPad8: ch = '8'; break;
                case Keys.NumPad9: ch = '9'; break;
                case Keys.Subtract: ch = '-'; break;
                case Keys.Add: ch = '+'; break;
                case Keys.Decimal: ch = '.'; break;
                case Keys.Divide: ch = '/'; break;
                case Keys.Multiply: ch = '*'; break;

                default:
                    if (key >= Keys.A && key <= Keys.Z)
                    {
                        // UC is 65-90  LC is 97-122
                        ch = iscap ? (char)(int)key : (char)(int)(key + 32);
                    }
                    break;
            }

            return (ch, keyCodes);
        }

        /// <summary>Key state query. Based on https://stackoverflow.com/a/9356006. </summary>
        /// <param name="key">Which key.</param>
        /// <returns></returns>
        public static KeyState GetKeyState(Keys key)
        {
            KeyState state = KeyState.NotPressed;

            short retVal = NativeMethods.GetKeyStateW32((int)key);

            // If the high-order bit is 1, the key is down otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
            {
                state = KeyState.Pressed;
            }

            // If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
            {
                state = KeyState.Toggled;
            }

            return state;
        }

        internal class NativeMethods
        {
            [Flags]
            public enum KeyStates { None = 0, Down = 1, Toggled = 2 }

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern short GetKeyStateW32(int keyCode);
        }
        */
    }
}
