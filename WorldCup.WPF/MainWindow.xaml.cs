using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WorldCup.DataLayer.Models;
using WorldCup.DataLayer.Services;
using WorldCup.WPF.Controls;
using WorldCup.WPF.Helpers;

namespace WorldCup.WPF;

public partial class MainWindow : Window
{
    private AppSettings     _settings;
    private readonly string _settingsPath;
    private readonly string _windowSizePath;
    private IDataService?   _dataService;
    private List<Team>      _allTeams   = new();
    private List<Match>     _allMatches = new();
    private string          _myFifaCode = string.Empty;
    private bool            _loading;
    private bool            _forceClose;

    public MainWindow(AppSettings settings, string settingsPath, string windowSizePath)
    {
        _settings       = settings;
        _settingsPath   = settingsPath;
        _windowSizePath = windowSizePath;
        InitializeComponent();
        fieldCanvas.Background = new DrawingBrush(CreateFieldDrawing());
        ApplyLanguage(_settings.Language);
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        => await LoadTeamsAsync();

    private async Task LoadTeamsAsync()
    {
        SetLoading(true);
        lblError.Visibility = Visibility.Collapsed;

        _loading = true;
        cbMyTeam.Items.Clear();
        cbOpponent.Items.Clear();
        lblResult.Text            = string.Empty;
        btnMyTeamInfo.IsEnabled   = false;
        btnOpponentInfo.IsEnabled = false;
        _loading = false;

        string? autoFifa = null;

        try
        {
            var (mode, jsonBasePath) = ConfigLoader.Load();
            _dataService = DataServiceFactory.Create(mode, jsonBasePath);
            _allTeams    = await _dataService.GetTeamsAsync(_settings.Gender);

            _loading = true;
            foreach (var team in _allTeams.OrderBy(t => t.Country))
                cbMyTeam.Items.Add($"{team.Country} ({team.FifaCode})");

            if (!string.IsNullOrEmpty(_settings.FavoriteTeamFifaCode))
            {
                var fav = _allTeams.FirstOrDefault(t => t.FifaCode == _settings.FavoriteTeamFifaCode);
                if (fav != null)
                {
                    string itemText = $"{fav.Country} ({fav.FifaCode})";
                    for (int i = 0; i < cbMyTeam.Items.Count; i++)
                    {
                        if ((string)cbMyTeam.Items[i] == itemText)
                        { cbMyTeam.SelectedIndex = i; break; }
                    }
                    autoFifa = fav.FifaCode;
                }
            }
            _loading = false;
        }
        catch (DataServiceException ex)
        {
            _loading = false;
            ShowError(ex.Message);
        }
        finally
        {
            SetLoading(false);
        }

        if (autoFifa != null)
            await LoadMatchesForTeamAsync(autoFifa);
    }

    private async void cbMyTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_loading) return;
        if (cbMyTeam.SelectedItem is not string selected) return;

        string fifaCode = ExtractFifaCode(selected);
        if (string.IsNullOrEmpty(fifaCode)) return;

        _settings.FavoriteTeamFifaCode = fifaCode;
        new SettingsService().Save(_settingsPath, _settings);

        await LoadMatchesForTeamAsync(fifaCode);
    }

    private async Task LoadMatchesForTeamAsync(string fifaCode)
    {
        if (_dataService == null) return;
        _myFifaCode = fifaCode;

        SetLoading(true);
        lblResult.Text            = string.Empty;
        btnOpponentInfo.IsEnabled = false;
        ClearField();

        _loading = true;
        cbOpponent.Items.Clear();
        _loading = false;

        try
        {
            _allMatches = await _dataService.GetMatchesByTeamAsync(_settings.Gender, fifaCode);

            var opponents = _allMatches
                .Select(m => m.HomeTeam.Code == fifaCode ? m.AwayTeam : m.HomeTeam)
                .GroupBy(ts => ts.Code)
                .Select(g => g.First())
                .OrderBy(ts => ts.Country)
                .ToList();

            _loading = true;
            foreach (var opp in opponents)
                cbOpponent.Items.Add($"{opp.Country} ({opp.Code})");
            _loading = false;

            btnMyTeamInfo.IsEnabled = true;
        }
        catch (DataServiceException ex)
        {
            _loading = false;
            ShowError(ex.Message);
        }
        finally
        {
            SetLoading(false);
        }
    }

    private void cbOpponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_loading) return;
        if (cbOpponent.SelectedItem is not string selected) return;

        string oppCode = ExtractFifaCode(selected);
        if (string.IsNullOrEmpty(oppCode)) return;

        btnOpponentInfo.IsEnabled = true;

        var match = _allMatches.FirstOrDefault(m =>
            (m.HomeTeam.Code == _myFifaCode && m.AwayTeam.Code == oppCode) ||
            (m.AwayTeam.Code == _myFifaCode && m.HomeTeam.Code == oppCode));

        if (match != null)
        {
            int myGoals  = match.HomeTeam.Code == _myFifaCode ? match.HomeTeam.Goals : match.AwayTeam.Goals;
            int oppGoals = match.HomeTeam.Code == _myFifaCode ? match.AwayTeam.Goals : match.HomeTeam.Goals;
            lblResult.Text = $"{myGoals} : {oppGoals}";
            PopulateField(match, oppCode);
        }
        else
        {
            lblResult.Text = string.Empty;
            ClearField();
        }
    }

    private void btnMyTeamInfo_Click(object sender, RoutedEventArgs e)
    {
        if (cbMyTeam.SelectedItem is not string selected) return;
        string code = ExtractFifaCode(selected);
        var team = _allTeams.FirstOrDefault(t => t.FifaCode == code);
        if (team == null) return;
        OpenTeamInfo(team);
    }

    private void btnOpponentInfo_Click(object sender, RoutedEventArgs e)
    {
        if (cbOpponent.SelectedItem is not string selected) return;
        string code = ExtractFifaCode(selected);
        var team = _allTeams.FirstOrDefault(t => t.FifaCode == code);
        if (team == null)
        {
            var ts = _allMatches
                .SelectMany(m => new[] { m.HomeTeam, m.AwayTeam })
                .FirstOrDefault(ts => ts.Code == code);
            if (ts == null) return;
            team = new WorldCup.DataLayer.Models.Team
            {
                Country  = ts.Country,
                FifaCode = ts.Code
            };
        }
        OpenTeamInfo(team);
    }

    private void OpenTeamInfo(WorldCup.DataLayer.Models.Team team)
    {
        var win = new TeamInfoWindow(team, _settings.Language);
        win.Owner = this;
        win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        win.Show();
    }

    private static readonly Dictionary<string, int> MyTeamRowY = new()
    {
        ["Goalie"]   = 40,
        ["Defender"] = 130,
        ["Midfield"] = 220,
        ["Forward"]  = 310
    };
    private static readonly Dictionary<string, int> OppTeamRowY = new()
    {
        ["Goalie"]   = 660,
        ["Defender"] = 570,
        ["Midfield"] = 480,
        ["Forward"]  = 390
    };
    private static readonly string[] PositionOrder = { "Goalie", "Defender", "Midfield", "Forward" };

    private void PopulateField(Match match, string oppCode)
    {
        fieldCanvas.Children.Clear();

        var myStats  = match.HomeTeam.Code == _myFifaCode ? match.HomeTeam : match.AwayTeam;
        var oppStats = match.HomeTeam.Code == _myFifaCode ? match.AwayTeam : match.HomeTeam;

        PlacePlayers(myStats,  _myFifaCode, isMyTeam: true,  fieldCanvas);
        PlacePlayers(oppStats, oppCode,     isMyTeam: false, fieldCanvas);

        fieldViewbox.Visibility = Visibility.Visible;
    }

    private void ClearField()
    {
        fieldCanvas.Children.Clear();
        fieldViewbox.Visibility = Visibility.Collapsed;
    }

    private void PlacePlayers(TeamStatistics stats, string fifaCode, bool isMyTeam, Canvas canvas)
    {
        const double fieldW  = 500;
        const double ctrlW   = 70;
        const double ctrlH   = 90;

        var rowY = isMyTeam ? MyTeamRowY : OppTeamRowY;
        var byPos = PositionOrder.ToDictionary(p => p, _ => new List<Player>());

        foreach (var player in stats.StartingEleven)
        {
            string pos = byPos.ContainsKey(player.Position) ? player.Position : "Forward";
            byPos[pos].Add(player);
        }

        foreach (string pos in PositionOrder)
        {
            var players = byPos[pos];
            if (players.Count == 0) continue;

            int rowCenterY = rowY[pos];
            int n = players.Count;

            for (int i = 0; i < n; i++)
            {
                double xCenter = fieldW / (n + 1) * (i + 1);
                double left    = xCenter - ctrlW / 2;
                double top     = rowCenterY - ctrlH / 2;

                var ctrl = new PlayerFieldControl();
                ctrl.SetPlayer(players[i], fifaCode, isMyTeam);
                ctrl.PlayerSelected += PlayerFieldControl_PlayerSelected;

                Canvas.SetLeft(ctrl, left);
                Canvas.SetTop(ctrl, top);
                canvas.Children.Add(ctrl);
            }
        }
    }

    private void PlayerFieldControl_PlayerSelected(Player player, string fifaCode)
    {
        if (cbOpponent.SelectedItem is not string oppSelected) return;
        string oppCode = ExtractFifaCode(oppSelected);
        if (string.IsNullOrEmpty(oppCode)) return;

        var match = _allMatches.FirstOrDefault(m =>
            (m.HomeTeam.Code == _myFifaCode && m.AwayTeam.Code == oppCode) ||
            (m.AwayTeam.Code == _myFifaCode && m.HomeTeam.Code == oppCode));
        if (match == null) return;

        var stats = match.HomeTeam.Code == fifaCode ? match.HomeTeam : match.AwayTeam;

        int goals = stats.Events.Count(ev =>
            ev.TypeOfEvent == "goal" &&
            ev.Player.Equals(player.Name, StringComparison.OrdinalIgnoreCase));

        int yellowCards = stats.Events.Count(ev =>
            ev.TypeOfEvent == "yellow-card" &&
            ev.Player.Equals(player.Name, StringComparison.OrdinalIgnoreCase));

        bool isMyTeam = fifaCode == _myFifaCode;

        var win = new PlayerDetailWindow(player, fifaCode, goals, yellowCards,
                                         _settings.Language, isMyTeam);
        win.Owner = this;
        win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        win.Show();
    }

    private static DrawingGroup CreateFieldDrawing()
    {
        var group    = new DrawingGroup();
        var whitePen = new Pen(Brushes.White, 2.5);
        var darkPen  = new Pen(new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF)), 1);

        group.Children.Add(new GeometryDrawing(
            new SolidColorBrush(Color.FromRgb(0x2E, 0x7D, 0x32)), null,
            new RectangleGeometry(new Rect(0, 0, 500, 700))));

        for (int row = 0; row < 7; row++)
        {
            if (row % 2 == 0) continue;
            group.Children.Add(new GeometryDrawing(
                new SolidColorBrush(Color.FromArgb(0x18, 0x00, 0x00, 0x00)), null,
                new RectangleGeometry(new Rect(0, row * 100, 500, 100))));
        }

        group.Children.Add(new GeometryDrawing(null, whitePen,
            new RectangleGeometry(new Rect(15, 15, 470, 670))));

        group.Children.Add(new GeometryDrawing(null, whitePen,
            new LineGeometry(new Point(15, 350), new Point(485, 350))));

        group.Children.Add(new GeometryDrawing(null, whitePen,
            new EllipseGeometry(new Point(250, 350), 60, 60)));

        group.Children.Add(new GeometryDrawing(Brushes.White, null,
            new EllipseGeometry(new Point(250, 350), 4, 4)));

        group.Children.Add(new GeometryDrawing(null, whitePen,
            new RectangleGeometry(new Rect(150, 15, 200, 100))));

        group.Children.Add(new GeometryDrawing(null, whitePen,
            new RectangleGeometry(new Rect(210, 15, 80, 35))));

        group.Children.Add(new GeometryDrawing(null, whitePen,
            new RectangleGeometry(new Rect(150, 585, 200, 100))));

        group.Children.Add(new GeometryDrawing(null, whitePen,
            new RectangleGeometry(new Rect(210, 650, 80, 35))));

        group.Children.Add(new GeometryDrawing(Brushes.White, null,
            new EllipseGeometry(new Point(250, 90), 3, 3)));
        group.Children.Add(new GeometryDrawing(Brushes.White, null,
            new EllipseGeometry(new Point(250, 610), 3, 3)));

        return group;
    }

    private void ForceShutdown() { _forceClose = true; Close(); }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_forceClose)
        {
            var s   = LocalizationHelper.GetStrings(_settings.Language);
            var dlg = new ConfirmDialog(s["exit_confirm"], s["exit_title"], s["yes"], s["no"]);
            dlg.Owner = this;
            if (dlg.ShowDialog() != true)
            {
                e.Cancel = true;
                return;
            }
        }
        base.OnClosing(e);
    }

    private void menuSettings_Click(object sender, RoutedEventArgs e) => OpenSettings();
    private void menuExit_Click(object sender, RoutedEventArgs e)     => Close();

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
        { OpenSettings(); e.Handled = true; }
        else if (e.Key == Key.Q && Keyboard.Modifiers == ModifierKeys.Control)
        { Close(); e.Handled = true; }
    }

    private async void OpenSettings()
    {
        var s        = LocalizationHelper.GetStrings(_settings.Language);
        string hint  = WindowSizeHelper.Load(_windowSizePath);

        var sw = new SettingsWindow(_settings, hint);
        sw.Owner = this;
        if (sw.ShowDialog() != true) return;

        var confirm = new ConfirmDialog(s["apply_settings_confirm"], s["settings"], s["yes"], s["no"]);
        confirm.Owner = this;
        if (confirm.ShowDialog() != true) return;

        _settings = new SettingsService().Load(_settingsPath);
        string newSizeLabel = WindowSizeHelper.Load(_windowSizePath);
        WindowSizeHelper.Apply(this, newSizeLabel);
        ClearField();
        ApplyLanguage(_settings.Language);
        await LoadTeamsAsync();
    }

    private void ApplyLanguage(string lang)
    {
        var s = LocalizationHelper.GetStrings(lang);
        Title               = s["main_title"];
        menuFile.Header     = s["file"];
        menuSettings.Header = s["settings"];
        menuExit.Header     = s["exit"];
        lblMyTeam.Text      = s["your_team"];
        lblOpponent.Text    = s["opponent"];
        btnMyTeamInfo.ToolTip   = s["team_info"];
        btnOpponentInfo.ToolTip = s["team_info"];
    }

    private void SetLoading(bool on)
        => pbLoading.Visibility = on ? Visibility.Visible : Visibility.Collapsed;

    private void ShowError(string message)
    {
        lblError.Text       = message;
        lblError.Visibility = Visibility.Visible;
    }

    private static string ExtractFifaCode(string item)
    {
        int start = item.LastIndexOf('(');
        int end   = item.LastIndexOf(')');
        if (start < 0 || end <= start) return string.Empty;
        return item.Substring(start + 1, end - start - 1);
    }
}
