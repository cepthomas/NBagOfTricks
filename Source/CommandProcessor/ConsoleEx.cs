
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NBagOfTricks.CommandProcessor
{
    /// <summary>
    /// Helpers for manipulating the system console, a creaky old win32 beast.
    /// </summary>
    public static class ConsoleEx
    {
        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Rectangle GetPosition()
        {
            IntPtr hWnd = GetConsoleWindow();

            var wp = WINDOWPLACEMENT.Default;
            GetWindowPlacement(hWnd, ref wp);

            Rectangle r = new Rectangle(
                wp.NormalPosition.Left,
                wp.NormalPosition.Top,
                wp.NormalPosition.Right - wp.NormalPosition.Left,
                wp.NormalPosition.Bottom - wp.NormalPosition.Top);

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        public static void SetPosition(Rectangle rect)
        {
            IntPtr hWnd = GetConsoleWindow();

            // Get information about the monitor.
            var mi = MONITORINFO.Default;
            GetMonitorInfo(MonitorFromWindow(hWnd, MONITOR_DEFAULTTOPRIMARY), ref mi);

            // Get information about this window's current placement. Apparently need to do this first.
            var wp = WINDOWPLACEMENT.Default;
            GetWindowPlacement(hWnd, ref wp);

            // Sanity checking.
            const int MIN_DIM = 200;
            int left = Math.Max(0, rect.Left);
            int right = Math.Min(mi.Work.Right, rect.Right);

            if(right - left < MIN_DIM)
            {
                right = left + MIN_DIM;
            }

            int top = Math.Max(0, rect.Top);
            int bottom = Math.Min(mi.Work.Bottom, rect.Bottom);

            if(bottom - top < MIN_DIM)
            {
                bottom = top + MIN_DIM;
            }

            wp.NormalPosition = new RECT()
            {
                Left = left,
                Top = top,
                Right = right,
                Bottom = bottom
            };

            SetWindowPlacement(hWnd, ref wp);
        }
        #endregion

        #region Interop
        const int MONITOR_DEFAULTTOPRIMARY = 1;
        const uint SW_RESTORE = 9;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [StructLayout(LayoutKind.Sequential)]
        struct MONITORINFO
        {
            public uint Size;
            public RECT Monitor;
            public RECT Work; // usable space excluding the taskbar
            public uint Flags;

            public static MONITORINFO Default
            {
                get { var inst = new MONITORINFO(); inst.Size = (uint)Marshal.SizeOf(inst); return inst; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x, y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct WINDOWPLACEMENT
        {
            public uint Length;
            public uint Flags;
            public uint ShowCmd;
            public POINT MinPosition;
            public POINT MaxPosition;
            public RECT NormalPosition;

            public static WINDOWPLACEMENT Default
            {
                get
                {
                    var instance = new WINDOWPLACEMENT();
                    instance.Length = (uint)Marshal.SizeOf(instance);
                    return instance;
                }
            }
        }
        #endregion
    }
}
