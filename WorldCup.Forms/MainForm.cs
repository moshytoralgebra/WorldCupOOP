using WorldCup.DataLayer.Models;
using WorldCup.DataLayer.Services;
using WorldCup.Forms.Controls;
using WorldCup.Forms.Helpers;

namespace WorldCup.Forms;

public partial class MainForm : Form
{
    private AppSettings     _settings;
    private readonly string _settingsPath;
    private IDataService?   _dataService;
    private List<Team>      _teams      = new();
    private List<Match>     _allMatches     = new();
    private string          _currentFifaCode = string.Empty;
    private bool            _loadingTeams;

    public MainForm(AppSettings settings, string settingsPath)
    {
        _settings     = settings;
        _settingsPath = settingsPath;
        InitializeComponent();
        ApplyLanguage(_settings.Language);
    }

    private async void MainForm_Load(object sender, EventArgs e) => await LoadTeamsAsync();

    private async Task LoadTeamsAsync()
    {
        pbLoading.Visible = true;
        cbTeams.Enabled   = false;
        lblError.Visible  = false;
        ClearPlayerPanels();

        string? autoFifa = null;

        try
        {
            var (mode, jsonBasePath) = ConfigLoader.Load();
            _dataService = DataServiceFactory.Create(mode, jsonBasePath);
            _teams = await _dataService.GetTeamsAsync(_settings.Gender);

            _loadingTeams = true;
            try
            {
                cbTeams.Items.Clear();
                foreach (var team in _teams.OrderBy(t => t.Country))
                    cbTeams.Items.Add($"{team.Country} ({team.FifaCode})");

                if (!string.IsNullOrEmpty(_settings.FavoriteTeamFifaCode))
                {
                    var fav = _teams.FirstOrDefault(t => t.FifaCode == _settings.FavoriteTeamFifaCode);
                    if (fav != null)
                    {
                        cbTeams.SelectedIndex = cbTeams.Items.IndexOf($"{fav.Country} ({fav.FifaCode})");
                        autoFifa = fav.FifaCode;
                    }
                }
            }
            finally
            {
                _loadingTeams = false;
            }

            cbTeams.Enabled = true;
        }
        catch (DataServiceException ex)
        {
            lblError.Text    = ex.Message;
            lblError.Visible = true;
        }
        finally
        {
            pbLoading.Visible = false;
        }

        if (autoFifa != null)
            LoadPlayersForTeam(autoFifa);
    }

    private void cbTeams_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_loadingTeams) return;
        if (cbTeams.SelectedItem is not string selected) return;

        var start = selected.LastIndexOf('(');
        var end   = selected.LastIndexOf(')');
        if (start < 0 || end <= start) return;

        var fifaCode = selected.Substring(start + 1, end - start - 1);
        _settings.FavoriteTeamFifaCode = fifaCode;
        new SettingsService().Save(_settingsPath, _settings);
        LoadPlayersForTeam(fifaCode);
    }

    private async void LoadPlayersForTeam(string fifaCode)
    {
        if (_dataService == null) return;
        _currentFifaCode = fifaCode;

        pbLoading.Visible = true;
        cbTeams.Enabled   = false;
        ClearPlayerPanels();
        ClearRankings();

        try
        {
            _allMatches = await _dataService.GetMatchesByTeamAsync(_settings.Gender, fifaCode);

            var match = _allMatches.FirstOrDefault();
            if (match != null)
            {
                var stats = match.HomeTeam.Code == fifaCode ? match.HomeTeam : match.AwayTeam;
                foreach (var player in stats.StartingEleven.Concat(stats.Substitutes))
                {
                    bool isFav = _settings.FavoritePlayerNames.Contains(player.Name);
                    var  card  = CreatePlayerCard(player, isFav, fifaCode);
                    (isFav ? flpFavorites : flpAllPlayers).Controls.Add(card);
                }
                UpdateFavoritesLabel();
            }

            await PopulateRankingsAsync(fifaCode);
        }
        catch (DataServiceException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ClearPlayerPanels();
            ClearRankings();
        }
        finally
        {
            pbLoading.Visible = false;
            cbTeams.Enabled   = true;
        }
    }

    private PlayerUserControl CreatePlayerCard(Player player, bool isFav, string fifaCode)
    {
        var card = new PlayerUserControl();
        card.SetPlayer(player, isFav, _settings.Language, fifaCode);
        card.Margin    = new Padding(6);
        card.AllowDrop = true;

        card.CardClicked             += PlayerCard_CardClicked;
        card.FavoriteToggleRequested += PlayerCard_FavoriteToggleRequested;
        card.DragEnter               += PlayerCard_DragEnter;
        card.DragDrop                += PlayerCard_DragDrop;
        card.ImageChanged            += PlayerCard_ImageChanged;

        return card;
    }

    private void PlayerCard_ImageChanged(object? sender, Player player) { }

    private async Task PopulateRankingsAsync(string fifaCode)
    {
        var data = await Task.Run(() => ComputePlayerStats(fifaCode));
        PopulateTopScorers(data.Goals, data.Appearances, data.Players, fifaCode);
        PopulateYellowCards(data.YellowCards, data.Appearances, data.Players, fifaCode);
        PopulateAttendance();
    }

    private sealed record PlayerStats(
        Dictionary<string, int>    Goals,
        Dictionary<string, int>    YellowCards,
        Dictionary<string, int>    Appearances,
        Dictionary<string, Player> Players);

    private PlayerStats ComputePlayerStats(string fifaCode)
    {
        var goals       = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var yellowCards = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var appearances = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var players     = new Dictionary<string, Player>(StringComparer.OrdinalIgnoreCase);

        foreach (var match in _allMatches)
        {
            var stats = match.HomeTeam.Code == fifaCode ? match.HomeTeam : match.AwayTeam;

            foreach (var p in stats.StartingEleven.Concat(stats.Substitutes))
            {
                appearances[p.Name] = appearances.GetValueOrDefault(p.Name) + 1;
                players.TryAdd(p.Name, p);
            }

            foreach (var ev in stats.Events)
            {
                switch (ev.TypeOfEvent)
                {
                    case "goal":
                        goals[ev.Player] = goals.GetValueOrDefault(ev.Player) + 1;
                        break;
                    case "yellow-card":
                        yellowCards[ev.Player] = yellowCards.GetValueOrDefault(ev.Player) + 1;
                        break;
                }
            }
        }

        return new PlayerStats(goals, yellowCards, appearances, players);
    }

    private void PopulateTopScorers(
        Dictionary<string, int>    goals,
        Dictionary<string, int>    appearances,
        Dictionary<string, Player> players,
        string fifaCode)
    {
        var sorted = goals
            .Where(kv => kv.Value > 0)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key)
            .ToList();

        var imgList = BuildImageList(sorted.Select(kv => kv.Key), players, fifaCode);
        var nameToIdx = BuildImageIndex(sorted.Select(kv => kv.Key), players);

        var oldList = lvTopScorers.SmallImageList;
        lvTopScorers.SmallImageList = imgList;
        oldList?.Dispose();

        lvTopScorers.Items.Clear();
        int rank = 1;
        foreach (var kv in sorted)
        {
            var item = new ListViewItem(rank.ToString(), nameToIdx.GetValueOrDefault(kv.Key, -1));
            item.SubItems.Add(kv.Key);
            item.SubItems.Add(kv.Value.ToString());
            item.SubItems.Add(appearances.GetValueOrDefault(kv.Key).ToString());
            lvTopScorers.Items.Add(item);
            rank++;
        }
    }

    private void PopulateYellowCards(
        Dictionary<string, int>    yellowCards,
        Dictionary<string, int>    appearances,
        Dictionary<string, Player> players,
        string fifaCode)
    {
        var sorted = yellowCards
            .Where(kv => kv.Value > 0)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key)
            .ToList();

        var imgList = BuildImageList(sorted.Select(kv => kv.Key), players, fifaCode);
        var nameToIdx = BuildImageIndex(sorted.Select(kv => kv.Key), players);

        var oldList = lvYellowCards.SmallImageList;
        lvYellowCards.SmallImageList = imgList;
        oldList?.Dispose();

        lvYellowCards.Items.Clear();
        int rank = 1;
        foreach (var kv in sorted)
        {
            var item = new ListViewItem(rank.ToString(), nameToIdx.GetValueOrDefault(kv.Key, -1));
            item.SubItems.Add(kv.Key);
            item.SubItems.Add(kv.Value.ToString());
            item.SubItems.Add(appearances.GetValueOrDefault(kv.Key).ToString());
            lvYellowCards.Items.Add(item);
            rank++;
        }
    }

    private void PopulateAttendance()
    {
        lvAttendance.Items.Clear();

        var sorted = _allMatches
            .Select(m => (m, att: ParseAttendance(m.Attendance)))
            .OrderByDescending(x => x.att)
            .ToList();

        int rank = 1;
        foreach (var (match, att) in sorted)
        {
            var item = new ListViewItem(rank.ToString());
            item.SubItems.Add(match.Location);
            item.SubItems.Add(att > 0 ? att.ToString("N0") : match.Attendance ?? "—");
            item.SubItems.Add(match.HomeTeamCountry);
            item.SubItems.Add(match.AwayTeamCountry);
            lvAttendance.Items.Add(item);
            rank++;
        }
    }

    private static int ParseAttendance(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return 0;
        var cleaned = raw.Replace(",", "").Replace(".", "").Replace(" ", "");
        return int.TryParse(cleaned, out int n) ? n : 0;
    }

    private static ImageList BuildImageList(
        IEnumerable<string>        names,
        Dictionary<string, Player> players,
        string fifaCode)
    {
        var list = new ImageList
        {
            ImageSize  = new Size(32, 32),
            ColorDepth = ColorDepth.Depth32Bit
        };

        foreach (var name in names)
        {
            int shirtNumber = players.TryGetValue(name, out var p) ? p.ShirtNumber : 0;
            list.Images.Add(PlayerImageHelper.LoadOrDefault(fifaCode, shirtNumber, new Size(32, 32)));
        }

        return list;
    }

    private static Dictionary<string, int> BuildImageIndex(
        IEnumerable<string>        names,
        Dictionary<string, Player> players)
    {
        var dict  = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        int index = 0;
        foreach (var name in names)
            dict[name] = index++;
        return dict;
    }

    private void ClearRankings()
    {
        lvTopScorers.Items.Clear();
        lvYellowCards.Items.Clear();
        lvAttendance.Items.Clear();

        var oldTop    = lvTopScorers.SmallImageList;
        var oldYellow = lvYellowCards.SmallImageList;
        lvTopScorers.SmallImageList  = null;
        lvYellowCards.SmallImageList = null;
        oldTop?.Dispose();
        oldYellow?.Dispose();
    }

    private void btnPrint_Click(object? sender, EventArgs e) => PrintRankings();

    private void PrintRankings()
    {
        if (string.IsNullOrEmpty(_currentFifaCode) || _allMatches.Count == 0)
        {
            bool hrCheck = _settings.Language == "hr";
            MessageBox.Show(
                hrCheck ? "Molimo odaberite reprezentaciju." : "Please select a team first.",
                hrCheck ? "Upozorenje" : "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        bool hr      = _settings.Language == "hr";
        var  team    = _teams.FirstOrDefault(t => t.FifaCode == _currentFifaCode);
        var  country = team?.Country ?? _currentFifaCode;
        var  today   = DateTime.Today.ToString("yyyy-MM-dd");

        using var dlg = new SaveFileDialog
        {
            Title    = hr ? "Spremi PDF poretka" : "Save Rankings PDF",
            Filter   = "PDF files|*.pdf",
            FileName = $"Rankings_{_currentFifaCode}_{today}.pdf"
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        try
        {
            RankingsPdfExporter.Export(
                dlg.FileName,
                _settings.Language,
                _settings.Gender,
                country,
                GetListViewData(lvTopScorers),
                GetListViewData(lvYellowCards),
                GetListViewData(lvAttendance));

            string successMsg = hr ? "PDF uspješno spremljen." : "PDF saved successfully.";
            string openMsg    = hr ? $"{successMsg}\n\nOtvoriti datoteku?" : $"{successMsg}\n\nOpen file?";

            if (MessageBox.Show(openMsg, hr ? "Uspjeh" : "Success",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"{(hr ? "Greška pri izvozu:" : "Export error:")} {ex.Message}",
                hr ? "Greška" : "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static List<string[]> GetListViewData(ListView lv) =>
        lv.Items.Cast<ListViewItem>()
            .Select(item => Enumerable.Range(0, item.SubItems.Count)
                .Select(i => item.SubItems[i].Text)
                .ToArray())
            .ToList();

    private void Flp_DragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = e.Data?.GetData(typeof(PlayerUserControl)) is PlayerUserControl
            ? DragDropEffects.Move : DragDropEffects.None;
    }

    private void FlpFavorites_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(typeof(PlayerUserControl)) is PlayerUserControl card)
            HandleDropToFavorites(card);
    }

    private void FlpAllPlayers_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(typeof(PlayerUserControl)) is PlayerUserControl card)
            HandleDropToAllPlayers(card);
    }

    private void PlayerCard_DragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = e.Data?.GetData(typeof(PlayerUserControl)) is PlayerUserControl
            ? DragDropEffects.Move : DragDropEffects.None;
    }

    private void PlayerCard_DragDrop(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetData(typeof(PlayerUserControl)) is not PlayerUserControl dropped) return;
        if (sender is not PlayerUserControl target) return;

        if (target.Parent == flpFavorites) HandleDropToFavorites(dropped);
        else if (target.Parent == flpAllPlayers) HandleDropToAllPlayers(dropped);
    }

    private void HandleDropToFavorites(PlayerUserControl card)
    {
        if (card.Parent == flpFavorites) return;
        if (FavCount() >= 3) { _ = FlashPanelAsync(flpFavorites); return; }
        MoveToFavorites(card);
        SaveFavorites();
    }

    private void HandleDropToAllPlayers(PlayerUserControl card)
    {
        if (card.Parent == flpAllPlayers) return;
        MoveToAllPlayers(card);
        SaveFavorites();
    }

    private void PlayerCard_FavoriteToggleRequested(object? sender, EventArgs e)
    {
        if (sender is not PlayerUserControl card) return;

        if (!card.IsFavorite)
        {
            if (FavCount() >= 3)
            {
                bool hr = _settings.Language == "hr";
                MessageBox.Show(
                    hr ? "Možete imati najviše 3 omiljena igrača." : "You can have at most 3 favorite players.",
                    hr ? "Ograničenje" : "Limit Reached",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MoveToFavorites(card);
        }
        else
        {
            MoveToAllPlayers(card);
        }

        SaveFavorites();
    }

    private void PlayerCard_CardClicked(object? sender, CardClickEventArgs e)
    {
        if (sender is not PlayerUserControl clicked) return;

        if (!e.IsCtrlHeld)
            foreach (var card in AllCards().Where(c => c != clicked))
                card.IsSelected = false;

        clicked.IsSelected = !clicked.IsSelected;
    }

    private void btnMoveFavs_Click(object? sender, EventArgs e)
    {
        var toMove = AllCards().Where(c => c.IsSelected && !c.IsFavorite).ToList();
        if (!toMove.Any()) return;

        int canAdd = 3 - FavCount();
        if (canAdd <= 0) { _ = FlashPanelAsync(flpFavorites); return; }

        foreach (var card in toMove.Take(canAdd))
            MoveToFavorites(card);

        SaveFavorites();
    }

    private void btnMoveAll_Click(object? sender, EventArgs e)
    {
        var toMove = AllCards().Where(c => c.IsSelected && c.IsFavorite).ToList();
        if (!toMove.Any()) return;

        foreach (var card in toMove)
            MoveToAllPlayers(card);

        SaveFavorites();
    }

    private void MoveToFavorites(PlayerUserControl card)
    {
        card.Parent?.Controls.Remove(card);
        flpFavorites.Controls.Add(card);
        card.IsFavorite = true;
        UpdateFavoritesLabel();
    }

    private void MoveToAllPlayers(PlayerUserControl card)
    {
        card.Parent?.Controls.Remove(card);
        flpAllPlayers.Controls.Add(card);
        card.IsFavorite = false;
        UpdateFavoritesLabel();
    }

    private void SaveFavorites()
    {
        _settings.FavoritePlayerNames = flpFavorites.Controls
            .OfType<PlayerUserControl>().Select(c => c.Player.Name).ToList();
        new SettingsService().Save(_settingsPath, _settings);
        UpdateFavoritesLabel();
    }

    private void ClearPlayerPanels()
    {
        foreach (var c in flpFavorites.Controls.OfType<PlayerUserControl>().ToArray()) c.Dispose();
        foreach (var c in flpAllPlayers.Controls.OfType<PlayerUserControl>().ToArray()) c.Dispose();
        flpFavorites.Controls.Clear();
        flpAllPlayers.Controls.Clear();
        UpdateFavoritesLabel();
    }

    private void UpdateFavoritesLabel()
    {
        bool hr = _settings.Language == "hr";
        lblFavTitle.Text = hr
            ? $"Omiljeni igrači ({FavCount()}/3)"
            : $"Favorite Players ({FavCount()}/3)";
    }

    private int FavCount() =>
        flpFavorites.Controls.OfType<PlayerUserControl>().Count();

    private IEnumerable<PlayerUserControl> AllCards() =>
        flpFavorites.Controls.OfType<PlayerUserControl>()
            .Concat(flpAllPlayers.Controls.OfType<PlayerUserControl>());

    private async Task FlashPanelAsync(FlowLayoutPanel panel)
    {
        var saved = panel.BackColor;
        panel.BackColor = Color.FromArgb(0xFF, 0xCC, 0xCC);
        await Task.Delay(300);
        panel.BackColor = saved;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        bool hr = _settings.Language == "hr";
        using var dlg = new ConfirmDialog(
            hr ? "Jeste li sigurni da želite izaći?" : "Are you sure you want to exit?",
            hr ? "Izlaz" : "Exit",
            hr ? "Da"   : "Yes",
            hr ? "Ne"   : "No");
        if (dlg.ShowDialog() != DialogResult.OK)
            e.Cancel = true;
        base.OnFormClosing(e);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Control | Keys.S: menuSettings_Click(this, EventArgs.Empty); return true;
            case Keys.Control | Keys.Q: Close(); return true;
            case Keys.Control | Keys.P: PrintRankings(); return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private async void menuSettings_Click(object sender, EventArgs e)
    {
        bool hr = _settings.Language == "hr";
        using var sf = new SettingsForm(_settingsPath, isFirstRun: false, currentSettings: _settings);
        if (sf.ShowDialog() != DialogResult.OK) return;

        using var confirm = new ConfirmDialog(
            hr ? "Primijeniti promjene? Podaci će se ponovno učitati." : "Apply changes? Data will be reloaded.",
            hr ? "Potvrda" : "Confirm",
            hr ? "Da"     : "Yes",
            hr ? "Ne"     : "No");
        if (confirm.ShowDialog() != DialogResult.OK) return;

        _settings = new SettingsService().Load(_settingsPath);
        ApplyLanguage(_settings.Language);
        await LoadTeamsAsync();
    }

    private void menuExit_Click(object sender, EventArgs e) => Close();

    private void ApplyLanguage(string lang)
    {
        bool hr = lang == "hr";

        Text               = hr ? "Svjetski Kup"                      : "World Cup";
        lblSelectTeam.Text = hr ? "Odaberite omiljenu reprezentaciju:" : "Select your favorite team:";
        menuFile.Text      = hr ? "Datoteka"  : "File";
        menuSettings.Text  = hr ? "Postavke"  : "Settings";
        menuExit.Text      = hr ? "Izlaz"     : "Exit";
        lblAllTitle.Text   = hr ? "Svi igrači": "All Players";
        btnMoveFavs.Text   = hr ? "Premjesti odabrane u favorite"    : "Move selected to favorites";
        btnMoveAll.Text    = hr ? "Premjesti odabrane u sve igrače"  : "Move selected to all players";

        tabPagePlayers.Text  = hr ? "Igrači"  : "Players";
        tabPageRankings.Text = hr ? "Poredak" : "Rankings";

        btnPrint.Text        = hr ? "Ispis poretka" : "Print Rankings";

        lblTopScorers.Text   = hr ? "Strijelci"    : "Top Scorers";
        lblYellowCards.Text  = hr ? "Žuti kartoni" : "Yellow Cards";
        lblAttendance.Text   = hr ? "Posjećenost"  : "Attendance";

        if (lvTopScorers.Columns.Count == 4)
        {
            lvTopScorers.Columns[0].Text = "#";
            lvTopScorers.Columns[1].Text = hr ? "Igrač"   : "Player";
            lvTopScorers.Columns[2].Text = hr ? "Golovi"  : "Goals";
            lvTopScorers.Columns[3].Text = hr ? "Nastupi" : "Appearances";
        }

        if (lvYellowCards.Columns.Count == 4)
        {
            lvYellowCards.Columns[0].Text = "#";
            lvYellowCards.Columns[1].Text = hr ? "Igrač"          : "Player";
            lvYellowCards.Columns[2].Text = hr ? "Žuti kartoni"   : "Yellow Cards";
            lvYellowCards.Columns[3].Text = hr ? "Nastupi"        : "Appearances";
        }

        if (lvAttendance.Columns.Count == 5)
        {
            lvAttendance.Columns[0].Text = "#";
            lvAttendance.Columns[1].Text = hr ? "Mjesto"       : "Location";
            lvAttendance.Columns[2].Text = hr ? "Posjećenost"  : "Attendance";
            lvAttendance.Columns[3].Text = hr ? "Domaćin"      : "Home Team";
            lvAttendance.Columns[4].Text = hr ? "Gost"         : "Away Team";
        }

        UpdateFavoritesLabel();
    }
}
