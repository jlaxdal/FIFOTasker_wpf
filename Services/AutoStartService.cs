using System;
using System.IO;
using Microsoft.Win32;

public class AutoStartService
{
    private const string AppName = "FIFOTasker";
    private readonly string _exePath;

    public AutoStartService()
    {
        _exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
    }

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
        return key?.GetValue(AppName) != null;
    }

    public void SetAutoStart(bool enable)
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

        if (enable)
        {
            key?.SetValue(AppName, _exePath);
        }
        else
        {
            key?.DeleteValue(AppName, false);
        }
    }
}