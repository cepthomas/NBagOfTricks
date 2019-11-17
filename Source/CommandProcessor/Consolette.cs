using NBagOfTricks.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NBagOfTricks.CommandProcessor
{
    /// <summary>
    /// Simple console with command history and a few other things.
    /// </summary>
    public class Consolette
    {
        #region Fields
        /// <summary>
        /// Recent commands. Most recent is at [0].
        /// </summary>
        List<string> _cmdHistory = new List<string>();

        /// <summary>
        /// Where we are in the command history.
        /// </summary>
        int _currentCmd = 0;

        /// <summary>
        /// History capacity.
        /// </summary>
        const int MAX_HISTORY = 20;

        /// <summary>
        /// Copy of current line.
        /// </summary>
        string _currentText = "";
        #endregion

        #region Properties
        /// <summary>
        /// What you like.
        /// </summary>
        public string Prompt { get; set; } = ">";

        /// <summary>
        /// The colors to display when text is matched.
        /// </summary>
        public Dictionary<string, ConsoleColor> Colors { get; set; } = new Dictionary<string, ConsoleColor>(); //  TODO.

        /// <summary>
        /// User selectable color.
        /// </summary>
        public ConsoleColor ForegroundColor { set { Console.ForegroundColor = value; } }

        /// <summary>
        /// User selectable color.
        /// </summary>
        public ConsoleColor BackgroundColor { set { Console.BackgroundColor = value; } }
        #endregion

        #region Events
        /// <summary>Device wants to say something.</summary>
        public class UserCommandArgs : EventArgs
        {
            /// <summary>Client handles this.</summary>
            public string Command { get; set; } = null;

            /// <summary>Client answers with this.</summary>
            public string Response { get; set; } = null;
        }
        public event EventHandler<UserCommandArgs> UserCommand;
        #endregion

        #region Public functions
        /// <summary>
        /// Constructor.
        /// </summary>
        public Consolette()
        {
            Console.SetWindowSize(1, 1);
            Console.SetBufferSize(100, 200);
            Console.SetWindowSize(100, 40);
            Console.Title = "Consolette";
        }

        /// <summary>
        /// Execute the loop forever.
        /// </summary>
        public void Run()
        {
            bool done = false;

            Console.Write(Prompt);

            void UpdateLine(string text)
            {
                // Clear current line.
                int current = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, current);
                // Write new.
                Console.Write(Prompt);
                Console.Write(text);
                _currentText = text;
            }

            while (!done)
            {
                var info = Console.ReadKey(true);

                switch (info.Key)
                {
                    case ConsoleKey.UpArrow:
                        // Get next oldest cmd and print it.
                        if (_currentCmd < _cmdHistory.Count)
                        {
                            UpdateLine(_cmdHistory[_currentCmd]);
                            _currentCmd = _currentCmd < _cmdHistory.Count - 1 ? _currentCmd + 1 : _currentCmd;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        // Get next newest cmd and print it.
                        if (_currentCmd > 0)
                        {
                            _currentCmd--;
                            UpdateLine(_cmdHistory[_currentCmd]);
                        }
                        else
                        {
                            UpdateLine("");
                            _currentCmd = 0;
                        }
                        break;

                    case ConsoleKey.Backspace:
                        if (_currentText.Length > 0)
                        {
                            UpdateLine(_currentText.Left(_currentText.Length - 1));
                        }
                        break;

                    case ConsoleKey.Enter:
                        // Execute current cmd.
                        if (_currentText.Length > 0)
                        {
                            _cmdHistory.UpdateMru(_currentText, MAX_HISTORY);
                            Console.Write(Environment.NewLine);
                            UserCommandArgs args = new UserCommandArgs() { Command = _currentText };
                            UserCommand?.Invoke(this, args);
                            Console.Write(args.Response);
                            Console.Write(Environment.NewLine);
                            UpdateLine("");
                        }
                        break;

                    case ConsoleKey k when k == ConsoleKey.X && info.Modifiers == ConsoleModifiers.Control:
                        done = true;
                        break;

                    default:
                        Console.Write(info.KeyChar);
                        _currentText += info.KeyChar;
                        break;
                }
            }
        }

        /// <summary>
        /// Remove everything.
        /// </summary>
        public void Clear()
        {
            Console.Clear();
            _cmdHistory.Clear();
            _currentCmd = 0;
        }
        #endregion
    }
}
