using WorldCup.DataLayer.Models;
using WorldCup.DataLayer.Services;

namespace WorldCup.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        string settingsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WorldCupApp");
        Directory.CreateDirectory(settingsFolder);
        string settingsPath = Path.Combine(settingsFolder, "settings.txt");
        var settingsService = new SettingsService();
        AppSettings settings;

        try
        {
            if (!settingsService.SettingsExist(settingsPath))
            {
                using var settingsForm = new SettingsForm(settingsPath, isFirstRun: true);
                if (settingsForm.ShowDialog() != DialogResult.OK)
                    return;
            }

            settings = settingsService.Load(settingsPath);
        }
        catch (SettingsException ex)
        {
            MessageBox.Show(ex.Message, "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new MainForm(settings, settingsPath));
    }
}
