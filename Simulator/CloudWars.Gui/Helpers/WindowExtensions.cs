using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace CloudWars.Helpers
{
    public static class FullScreenHelper
    {
        // Nearest monitor to window
        private const int monitorDefaultToNearest = 2;

        public static bool IsFullscreen(this Window window)
        {
            return (window.WindowStyle == WindowStyle.None &&
                    window.ResizeMode == ResizeMode.NoResize &&
                    window.WindowState == WindowState.Maximized);
        }

        public static void SetFullscreen(this Window window)
        {
            // Make window borderless
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;

            // Get handle for nearest monitor to this window
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);
            IntPtr nearestMonitor = MonitorFromWindow(windowInteropHelper.Handle, monitorDefaultToNearest);

            // Get monitor info
            MONITORINFOEX monitorInfo = new MONITORINFOEX();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            GetMonitorInfo(new HandleRef(window, nearestMonitor), monitorInfo);

            // Create working area dimensions, converted to DPI-independent values
            HwndSource source = HwndSource.FromHwnd(windowInteropHelper.Handle);
            if (source == null || source.CompositionTarget == null)
                return;

            Matrix matrix = source.CompositionTarget.TransformFromDevice;
            RECT workingArea = monitorInfo.rcMonitor;
            Point dpiIndependentSize = matrix.Transform(new Point(workingArea.Right - workingArea.Left,
                                                                  workingArea.Bottom - workingArea.Top));

            window.MaxWidth = dpiIndependentSize.X;
            window.MaxHeight = dpiIndependentSize.Y;
            window.WindowState = WindowState.Maximized;
        }

        // To get a handle to the specified monitor
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        // To get the working area of the specified monitor
        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX monitorInfo);

        // Monitor information (used by GetMonitorInfo())

        #region Nested type: MONITORINFOEX

        [StructLayout(LayoutKind.Sequential)]
        public class MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor; // Total area
            public RECT rcWork; // Working area
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)] public char[] szDevice;
        }

        #endregion

        // Rectangle (used by MONITORINFOEX)

        #region Nested type: RECT

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #endregion
    }
}