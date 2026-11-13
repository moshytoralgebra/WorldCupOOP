using System.IO;
using System.Windows;
using WorldCup.DataLayer.Services;
using WorldCup.WPF.Helpers;

namespace WorldCup.WPF;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            string folder       = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WorldCupApp");
            Directory.CreateDirectory(folder);
            string settingsPath   = Path.Combine(folder, "settings.txt");
            string windowSizePath = Path.Combine(folder, "windowsize.txt");

            var svc = new SettingsService();

            if (!svc.SettingsExist(settingsPath))
            {
                var sw = new SettingsWindow(settingsPath, windowSizePath);
                if (sw.ShowDialog() != true)
                {
                    Shutdown();
                    return;
                }
            }

            var settings  = svc.Load(settingsPath);
            string sizeLabel = WindowSizeHelper.Load(windowSizePath);

            var main = new MainWindow(settings, settingsPath, windowSizePath);
            WindowSizeHelper.Apply(main, sizeLabel);
            main.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}
