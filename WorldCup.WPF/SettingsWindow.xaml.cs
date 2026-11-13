using System.IO;
using System.Windows;
using System.Windows.Controls;
using WorldCup.DataLayer.Models;
using WorldCup.DataLayer.Services;
using WorldCup.WPF.Helpers;

namespace WorldCup.WPF;

public partial class SettingsWindow : Window
{
    private readonly string _settingsPath;
    private readonly string _windowSizePath;
    private readonly bool   _isFirstRun;
    private bool            _suppressEvents;

    public SettingsWindow(string settingsPath, string windowSizePath)
    {
        _settingsPath   = settingsPath;
        _windowSizePath = windowSizePath;
        _isFirstRun     = true;
        InitializeComponent();
        ApplyLanguage("en");
    }

    public SettingsWindow(AppSettings current, string? windowSizeHint = null)
    {
        string folder   = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WorldCupApp");
        _settingsPath   = Path.Combine(folder, "settings.txt");
        _windowSizePath = Path.Combine(folder, "windowsize.txt");
        _isFirstRun     = false;
        InitializeComponent();
        PrePopulate(current, windowSizeHint ?? WindowSizeHelper.Load(_windowSizePath));
    }

    private void PrePopulate(AppSettings s, string sizeLabel)
    {
        _suppressEvents = true;
        try
        {
            if (s.Gender == "men")        rbMale.IsChecked   = true;
            else if (s.Gender == "women") rbFemale.IsChecked = true;

            foreach (ComboBoxItem item in cbLanguage.Items)
                if ((string)item.Tag == s.Language) { cbLanguage.SelectedItem = item; break; }

            foreach (ComboBoxItem item in cbWindowSize.Items)
                if ((string)item.Tag == sizeLabel) { cbWindowSize.SelectedItem = item; break; }

            ApplyLanguage(s.Language);
        }
        finally
        {
            _suppressEvents = false;
        }
        UpdateConfirmState();
    }

    private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressEvents) return;
        if (cbLanguage.SelectedItem is ComboBoxItem item)
            ApplyLanguage((string)item.Tag);
        UpdateConfirmState();
    }

    private void cbWindowSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressEvents) return;
        UpdateConfirmState();
    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e) => UpdateConfirmState();

    private void btnConfirm_Click(object sender, RoutedEventArgs e)
    {
        string lang = cbLanguage.SelectedItem is ComboBoxItem li ? (string)li.Tag : "en";
        var settings = new AppSettings
        {
            Gender   = rbMale.IsChecked == true ? "men" : "women",
            Language = lang
        };
        new SettingsService().Save(_settingsPath, settings);

        string sizeTag = cbWindowSize.SelectedItem is ComboBoxItem si
            ? (string)si.Tag : "Medium (1024x768)";
        File.WriteAllText(_windowSizePath, sizeTag);

        DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        if (_isFirstRun) Application.Current.Shutdown();
    }

    private void UpdateConfirmState()
    {
        if (btnConfirm == null) return;
        bool genderOk = rbMale.IsChecked == true || rbFemale.IsChecked == true;
        bool langOk   = cbLanguage.SelectedIndex  >= 0;
        bool sizeOk   = cbWindowSize.SelectedIndex >= 0;
        btnConfirm.IsEnabled = genderOk && langOk && sizeOk;
    }

    private void ApplyLanguage(string lang)
    {
        var s = LocalizationHelper.GetStrings(lang);
        Title                 = s["settings_title"];
        lblChampionship.Text  = s["championship"];
        rbMale.Content        = s["male"];
        rbFemale.Content      = s["female"];
        lblLanguage.Text      = s["language"];
        lblWindowSize.Text    = s["window_size"];
        cbiFullscreen.Content = s["fullscreen"];
        cbiLarge.Content      = s["window_large"];
        cbiMedium.Content     = s["window_medium"];
        cbiSmall.Content      = s["window_small"];
        btnConfirm.Content    = s["confirm"];
        btnCancel.Content     = s["cancel"];
    }
}
