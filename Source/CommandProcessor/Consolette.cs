using NBagOfTricks.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace NBagOfTricks.CommandProcessor
{
    ///// <summary>
    ///// Simple console with command history and a few other things. TODO all broken....
    ///// </summary>
    //public class Consolette
    //{
    //    #region Fields
    //    /// <summary>
    //    /// Recent commands. Most recent is at [0].
    //    /// </summary>
    //    List<string> _cmdHistory = new List<string>();

    //    /// <summary>
    //    /// Where we are in the command history.
    //    /// </summary>
    //    int _currentCmd = 0;

    //    /// <summary>
    //    /// History capacity.
    //    /// </summary>
    //    const int MAX_HISTORY = 20;

    //    /// <summary>
    //    /// Copy of current line.
    //    /// </summary>
    //    string _currentText = "";

    //    /// <summary>
    //    /// Default value.
    //    /// </summary>
    //    int _width = 100;

    //    /// <summary>
    //    /// Default value.
    //    /// </summary>
    //    int _height = 50;

    //    /// <summary>
    //    /// Default value.
    //    /// </summary>
    //    int _x = 50;

    //    /// <summary>
    //    /// Default value.
    //    /// </summary>
    //    int _y = 50;
    //    #endregion

    //    #region Properties
    //    /// <summary>
    //    /// What you like.
    //    /// </summary>
    //    public string Prompt { get; set; } = ">";

    //    /// <summary>
    //    /// The colors to display when text is matched.
    //    /// </summary>
    //    public Dictionary<string, ConsoleColor> Colors { get; set; } = new Dictionary<string, ConsoleColor>(); //  TODO.

    //    /// <summary>
    //    /// What you like.
    //    /// </summary>
    //    public int Width { get { return _width; } set { _width = value; SetSize(); } }

    //    /// <summary>
    //    /// What you like.
    //    /// </summary>
    //    public int Height { get { return _height; } set { _height = value; SetSize(); } }

    //    /// <summary>
    //    /// What you like.
    //    /// </summary>
    //    public int X { get { return _x; } set { _x = value; SetPosition(); } }

    //    /// <summary>
    //    /// What you like.
    //    /// </summary>
    //    public int Y { get { return _y; } set { _y = value; SetPosition(); } }

    //    /// <summary>
    //    /// User selectable color.
    //    /// </summary>
    //    public ConsoleColor ForegroundColor { set { Console.ForegroundColor = value; } }

    //    /// <summary>
    //    /// User selectable color.
    //    /// </summary>
    //    public ConsoleColor BackgroundColor { set { Console.BackgroundColor = value; } }
    //    #endregion

    //    #region Events
    //    /// <summary>Device wants to say something.</summary>
    //    public class UserCommandArgs : EventArgs
    //    {
    //        /// <summary>Client handles this.</summary>
    //        public string Command { get; set; } = null;

    //        /// <summary>Client answers with this.</summary>
    //        public string Response { get; set; } = null;
    //    }
    //    public event EventHandler<UserCommandArgs> UserCommand;
    //    #endregion

    //    #region Public functions
    //    /// <summary>
    //    /// Constructor.
    //    /// </summary>
    //    public Consolette()
    //    {
    //        Console.Title = "Consolette";
    //        SetSize();
    //        SetPosition();
    //    }

    //    /// <summary>
    //    /// Execute the loop forever.
    //    /// </summary>
    //    public void Run()
    //    {
    //        bool done = false;

    //        Console.Write(Prompt);

    //        void UpdateLine(string text)
    //        {
    //            // Clear current line.
    //            int current = Console.CursorTop;
    //            Console.SetCursorPosition(0, Console.CursorTop);
    //            Console.Write(new string(' ', Console.WindowWidth));
    //            Console.SetCursorPosition(0, current);
    //            // Write new.
    //            Console.Write(Prompt);
    //            Console.Write(text);
    //            _currentText = text;
    //        }

    //        while (!done)
    //        {
    //            var info = Console.ReadKey(true);

    //            switch (info.Key)
    //            {
    //                case ConsoleKey.UpArrow:
    //                    // Get next oldest cmd and print it.
    //                    if (_currentCmd < _cmdHistory.Count)
    //                    {
    //                        UpdateLine(_cmdHistory[_currentCmd]);
    //                        _currentCmd = _currentCmd < _cmdHistory.Count - 1 ? _currentCmd + 1 : _currentCmd;
    //                    }
    //                    break;

    //                case ConsoleKey.DownArrow:
    //                    // Get next newest cmd and print it.
    //                    if (_currentCmd > 0)
    //                    {
    //                        _currentCmd--;
    //                        UpdateLine(_cmdHistory[_currentCmd]);
    //                    }
    //                    else
    //                    {
    //                        UpdateLine("");
    //                        _currentCmd = 0;
    //                    }
    //                    break;

    //                case ConsoleKey.Backspace:
    //                    if (_currentText.Length > 0)
    //                    {
    //                        UpdateLine(_currentText.Left(_currentText.Length - 1));
    //                    }
    //                    break;

    //                case ConsoleKey.Enter:
    //                    // Execute current cmd.
    //                    if (_currentText.Length > 0)
    //                    {
    //                        _cmdHistory.UpdateMru(_currentText, MAX_HISTORY);
    //                        Console.Write(Environment.NewLine);
    //                        UserCommandArgs args = new UserCommandArgs() { Command = _currentText };
    //                        UserCommand?.Invoke(this, args);
    //                        Console.Write(args.Response);
    //                        Console.Write(Environment.NewLine);
    //                        UpdateLine("");
    //                    }
    //                    break;

    //                //case ConsoleKey k when k == ConsoleKey.X && info.Modifiers == ConsoleModifiers.Control:
    //                //    done = true;
    //                //    break;

    //                default:
    //                    Console.Write(info.KeyChar);
    //                    _currentText += info.KeyChar;
    //                    break;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Remove everything.
    //    /// </summary>
    //    public void Clear()
    //    {
    //        Console.Clear();
    //        _cmdHistory.Clear();
    //        _currentCmd = 0;
    //    }
    //    #endregion




    //    #region Interop

    //    [DllImport("kernel32.dll")]
    //    static extern IntPtr GetConsoleWindow();

    //    [DllImport("user32.dll")]
    //    static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    //    const int MONITOR_DEFAULTTOPRIMARY = 1;

    //    [DllImport("user32.dll")]
    //    static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    //    [StructLayout(LayoutKind.Sequential)]
    //    struct MONITORINFO
    //    {
    //        public uint cbSize;
    //        public RECT rcMonitor;
    //        public RECT rcWork;
    //        public uint dwFlags;
    //        public static MONITORINFO Default
    //        {
    //            get { var inst = new MONITORINFO(); inst.cbSize = (uint)Marshal.SizeOf(inst); return inst; }
    //        }
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    struct RECT
    //    {
    //        public int Left, Top, Right, Bottom;
    //    }

    //    [StructLayout(LayoutKind.Sequential)]
    //    struct POINT
    //    {
    //        public int x, y;
    //    }

    //    [DllImport("user32.dll", SetLastError = true)]
    //    static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    //    [DllImport("user32.dll", SetLastError = true)]
    //    static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

    //    const uint SW_RESTORE = 9;

    //    [StructLayout(LayoutKind.Sequential)]
    //    struct WINDOWPLACEMENT
    //    {
    //        public uint Length;
    //        public uint Flags;
    //        public uint ShowCmd;
    //        public POINT MinPosition;
    //        public POINT MaxPosition;
    //        public RECT NormalPosition;
    //        public static WINDOWPLACEMENT Default
    //        {
    //            get
    //            {
    //                var instance = new WINDOWPLACEMENT();
    //                instance.Length = (uint)Marshal.SizeOf(instance);
    //                return instance;
    //            }
    //        }
    //    }

    //    #endregion


    //    public static void MainXXX()
    //    {
    //        // Get this console window's hWnd (window handle).
    //        IntPtr hWnd = GetConsoleWindow();

    //        // Get information about the monitor (display) that the window is (mostly) displayed on.
    //        // The .rcWork field contains the monitor's work area, i.e., the usable space excluding
    //        // the taskbar (and "application desktop toolbars" - see https://msdn.microsoft.com/en-us/library/windows/desktop/ms724947(v=vs.85).aspx)
    //        var mi = MONITORINFO.Default;
    //        GetMonitorInfo(MonitorFromWindow(hWnd, MONITOR_DEFAULTTOPRIMARY), ref mi);

    //        // Get information about this window's current placement.
    //        var wp = WINDOWPLACEMENT.Default;
    //        GetWindowPlacement(hWnd, ref wp);

    //        // Calculate the window's new position: lower left corner.
    //        // !! Inexplicably, on W10, work-area coordinates (0,0) appear to be (7,7) pixels 
    //        // !! away from the true edge of the screen / taskbar.
    //        int fudgeOffset = 7;
    //        wp.NormalPosition = new RECT()
    //        {
    //            Left = -fudgeOffset,
    //            Top = mi.rcWork.Bottom - (wp.NormalPosition.Bottom - wp.NormalPosition.Top),
    //            Right = (wp.NormalPosition.Right - wp.NormalPosition.Left),
    //            Bottom = fudgeOffset + mi.rcWork.Bottom
    //        };


    //        wp.NormalPosition = new RECT()
    //        {
    //            Left = 100,
    //            Top = 50,
    //            Right = 600,
    //            Bottom = 400
    //        };


    //        // Place the window at the new position.
    //        SetWindowPlacement(hWnd, ref wp);

    //    }







    //    #region Private functions



    //    // TODO also check limits
    //    // The BufferHeight and BufferWidth property gets/sets the number of rows and columns to be displayed.

    //    //WindowHeight and WindowWidth properties must always be less than BufferHeight and BufferWidth respectively.

    //    //WindowLeft must be less than BufferWidth - WindowWidth and WindowTop must be less than BufferHeight - WindowHeight.

    //    //WindowLeft and WindowTop are relative to the buffer.



    //    /// <summary>
    //    /// Helper.
    //    /// </summary>
    //    void SetSize()
    //    {
    //        //Console.SetWindowSize(1, 1);
    //        //Console.SetBufferSize(_width, 200);
    //        //Console.SetWindowSize(_width, _height);
    //    }

    //    /// <summary>
    //    /// Helper.
    //    /// </summary>
    //    void SetPosition()
    //    {
    //        // Console.SetWindowPosition(_x, _y);
    //    }
    //    #endregion
    //}
}
