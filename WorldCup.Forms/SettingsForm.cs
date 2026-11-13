using WorldCup.DataLayer.Models;
using WorldCup.DataLayer.Services;

namespace WorldCup.Forms;

public partial class SettingsForm : Form
{
    private readonly string _settingsPath;
    private readonly bool _isFirstRun;

    public SettingsForm(string settingsPath, bool isFirstRun = false, AppSettings? currentSettings = null)
    {
        _settingsPath = settingsPath;
        _isFirstRun = isFirstRun;
        InitializeComponent();
        if (currentSettings != null)
            PrePopulate(currentSettings);
    }

    private void PrePopulate(AppSettings settings)
    {
        cbLanguage.SelectedIndex = settings.Language == "hr" ? 1 : 0;
        if (settings.Gender == "men")        rbMale.Checked   = true;
        else if (settings.Gender == "women") rbFemale.Checked = true;
    }

    private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        ApplyLanguage(cbLanguage.SelectedIndex == 1 ? "hr" : "en");
        UpdateConfirmState();
    }

    private void rbGender_CheckedChanged(object sender, EventArgs e) => UpdateConfirmState();

    private void btnConfirm_Click(object sender, EventArgs e)
    {
        var settings = new AppSettings
        {
            Gender = rbMale.Checked ? "men" : "women",
            Language = cbLanguage.SelectedIndex == 1 ? "hr" : "en"
        };
        new SettingsService().Save(_settingsPath, settings);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        if (_isFirstRun)
        {
            Application.Exit();
        }
        else
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    private void UpdateConfirmState()
    {
        btnConfirm.Enabled = (rbMale.Checked || rbFemale.Checked) && cbLanguage.SelectedIndex >= 0;
    }

    private void ApplyLanguage(string lang)
    {
        bool hr = lang == "hr";
        Text = hr ? "Postavke" : "Settings";
        lblGender.Text = hr ? "Natjecanje" : "Championship";
        rbMale.Text = hr ? "Muško (2018)" : "Male (2018)";
        rbFemale.Text = hr ? "Žensko (2019)" : "Female (2019)";
        lblLanguage.Text = hr ? "Jezik" : "Language";
        btnConfirm.Text = hr ? "Potvrdi" : "Confirm";
        btnCancel.Text = hr ? "Odustani" : "Cancel";
    }
}
