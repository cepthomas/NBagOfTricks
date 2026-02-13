using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;


// WBOT is experiments for manipulating consoles. Needs more debugging.
// Borrowed from https://developercommunity.visualstudio.com/t/console-output-is-gone-in-vs2017-works-fine-when-d/12166


namespace Ephemera.NBagOfTricks
{
    public sealed class ConsoleUtils
    {
        #region Api
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void Move(int x, int y, int width, int height)
        {
            IntPtr hnd = GetForegroundWindow();
            MoveWindow(hnd, x, y, width, height, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Rectangle GetRect()
        {
            IntPtr hnd = GetForegroundWindow();
            GetWindowRect(hnd, out RectNative nrect);
            return new Rectangle(nrect.Left, nrect.Top, nrect.Right - nrect.Left, nrect.Bottom - nrect.Top);
        }

        /// <summary>
        /// From WBOT TBD.
        /// </summary>
        public static void Hide()
        {
            FreeConsole();
        }

        /// <summary>
        /// From WBOT TBD.
        /// </summary>
        /// <param name="stdOut"></param>
        /// <param name="stdIn"></param>
        /// <param name="stdErr"></param>
        public static void UnredirectConsole(out IntPtr stdOut, out IntPtr stdIn, out IntPtr stdErr)
        {
            SetStdHandle(STD_OUTPUT_HANDLE, stdOut = GetConsoleStandardOutput());
            SetStdHandle(STD_INPUT_HANDLE, stdIn = GetConsoleStandardInput());
            SetStdHandle(STD_ERROR_HANDLE, stdErr = GetConsoleStandardError());
        }
        #endregion

        #region Internal
        /// <summary>
        /// From WBOT TBD.
        /// </summary>
        /// <returns></returns>
        static IntPtr GetConsoleStandardInput()
        {
            var handle = CreateFile("CONIN$", GENERIC_READ | GENERIC_WRITE,
                FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            return handle; // InvalidHandleValue
        }

        /// <summary>
        /// From WBOT TBD.
        /// </summary>
        /// <returns></returns>
        static IntPtr GetConsoleStandardOutput()
        {
            var handle = CreateFile("CONOUT$", GENERIC_WRITE,
                FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            return handle; // InvalidHandleValue
        }

        /// <summary>
        /// From WBOT TBD.
        /// </summary>
        /// <returns></returns>
        static IntPtr GetConsoleStandardError()
        {
            var handle = CreateFile("CONERR$", GENERIC_WRITE,
                FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            return handle; // InvalidHandleValue
        }
        #endregion

        #region Native Win32
        struct RectNative
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public struct COORD
        {
            public short X;
            public short Y;
        };

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint GENERIC_EXECUTE = 0x20000000;
        const uint GENERIC_ALL = 0x10000000;
        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPUT_HANDLE = -11;
        const int STD_ERROR_HANDLE = -12;
        const int MY_CODE_PAGE = 437;

        static readonly IntPtr InvalidHandleValue = new(-1);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RectNative lpRect);

        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        // ===== Below is from WBOT TBD =========
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int AllocConsole();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, COORD size);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DetachConsole();

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateFile(string lpFileName, [MarshalAs(UnmanagedType.U4)] uint dwDesiredAccess, [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode, IntPtr lpSecurityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);
        #endregion

        #region Leftovers
        //// Setting window row/column doesn't work in .NET so take a dive into win32.
        //public int WindowLeft
        //{
        //    get { var r = GetRect(); return r.X; }
        //    set { var r = GetRect(); Move(value, r.Y, r.Width, r.Height); }
        //}

        //public int WindowTop
        //{
        //    get { var r = GetRect(); return r.Top; }
        //    set { var r = GetRect(); Move(r.X, value, r.Width, r.Height); }
        //}

        //public int WindowWidth
        //{
        //    get { var r = GetRect(); return r.Width; }
        //    set { var r = GetRect(); Move(r.X, r.Y, value, r.Height); }
        //}

        //public int WindowHeight
        //{
        //    get { var r = GetRect(); return r.Height; }
        //    set { var r = GetRect(); Move(r.X, r.Y, r.Width, value); }
        //}

        //public static void Show(int bufferWidth = -1, bool breakRedirection = true, int bufferHeight = 1600, int screenNum = -1 /*-1 = Any but primary*/)
        //{
        //    AllocConsole();
        //    IntPtr stdOut = InvalidHandleValue;
        //    if (breakRedirection)
        //    {
        //        UnredirectConsole(out stdOut, out nint stdIn, out nint stdErr);
        //    }

        //    var outStream = Console.OpenStandardOutput();
        //    var errStream = Console.OpenStandardError();
        //    Encoding encoding = Encoding.GetEncoding(0); //was MY_CODE_PAGE

        //    StreamWriter standardOutput = new(outStream, encoding), standardError = new(errStream, encoding);
        //    Screen? screen = null;

        //    try
        //    {
        //        screen = screenNum < 0 ?
        //            Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault() :
        //            Screen.AllScreens[Math.Min(screenNum, Screen.AllScreens.Length - 1)];
        //    }
        //    catch (Exception e)
        //    {
        //        // was ignored
        //        Debug.WriteLine(e.Message);
        //    }

        //    if (bufferWidth == -1)
        //    {
        //        if (screen == null)
        //        {
        //            bufferWidth = 180;
        //        }
        //        else
        //        {
        //            var bwid = screen.WorkingArea.Width / 10;
        //            bufferWidth = bwid > 15 ? bwid - 5 : bwid + 10;
        //        }
        //    }

        //    try
        //    {
        //        standardOutput.AutoFlush = true;
        //        standardError.AutoFlush = true;
        //        Console.SetOut(standardOutput);
        //        Console.SetError(standardError);

        //        if (breakRedirection)
        //        {
        //            var coord = new COORD
        //            {
        //                X = (short)bufferWidth,
        //                Y = (short)bufferHeight
        //            };
        //            SetConsoleScreenBufferSize(stdOut, coord);
        //        }
        //        else
        //        {
        //            Console.SetBufferSize(bufferWidth, bufferHeight);
        //        }
        //    }
        //    catch (Exception e) // Could be redirected
        //    {
        //        Debug.WriteLine(e.ToString());
        //    }

        //    try
        //    {
        //        if (screen != null)
        //        {
        //            var workingArea = screen.WorkingArea;
        //            IntPtr hConsole = GetConsoleWindow();
        //            WM.MoveWindow(hConsole, new(workingArea.Left, workingArea.Top));
        //            WM.ResizeWindow(hConsole, new(workingArea.Width, workingArea.Height));
        //        }
        //    }
        //    catch (Exception e) // Could be redirected
        //    {
        //        Debug.WriteLine(e.ToString());
        //    }
        //}
        #endregion
    }
}
