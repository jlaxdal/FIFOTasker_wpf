using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

public static class WindowHelper
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private const int DWMWA_NCRENDERING_POLICY = 2;   // Disables non-client rendering (removes shadow)

    public static void RemoveWindowShadow(Window window)
    {
        if (window == null) return;

        var helper = new WindowInteropHelper(window);
        if (helper.Handle == IntPtr.Zero)
            window.SourceInitialized += (s, e) => RemoveWindowShadow(window);
        else
        {
            int policy = 2; // DWMNCRP_DISABLED
            DwmSetWindowAttribute(helper.Handle, DWMWA_NCRENDERING_POLICY, ref policy, sizeof(int));
        }
    }

    public static void PositionMainWindow(Window window)
    {
        var workingArea = SystemParameters.WorkArea;

        // Bottom-right, sitting just above the taskbar
        window.Left = workingArea.Right - window.Width - 12;   // small margin from right edge
        window.Top = workingArea.Bottom - window.Height - 12;  // small margin above taskbar
    }
}