using System.IO;
using System.Windows;

namespace WorldCup.WPF.Helpers;

internal static class WindowSizeHelper
{
    private const string DefaultLabel = "Medium (1024x768)";

    public static void Apply(Window window, string sizeLabel)
    {
        if (sizeLabel.Contains("1280"))
        {
            window.WindowState = WindowState.Normal;
            window.Width       = 1280;
            window.Height      = 720;
        }
        else if (sizeLabel.Contains("800"))
        {
            window.WindowState = WindowState.Normal;
            window.Width       = 800;
            window.Height      = 600;
        }
        else if (sizeLabel.Contains("Fullscreen") || sizeLabel.Contains("zaslon"))
        {
            window.WindowStyle = WindowStyle.SingleBorderWindow;
            window.WindowState = WindowState.Maximized;
        }
        else
        {
            window.WindowState = WindowState.Normal;
            window.Width       = 1024;
            window.Height      = 768;
        }
    }

    public static string Load(string filePath)
    {
        if (!File.Exists(filePath)) return DefaultLabel;
        string text = File.ReadAllText(filePath).Trim();
        return string.IsNullOrEmpty(text) ? DefaultLabel : text;
    }
}
