using WorldCup.DataLayer.Models;

namespace WorldCup.DataLayer.Services;

public class FavoritesRepository
{
    private readonly SettingsService _settingsService;
    private readonly string _filePath;

    public FavoritesRepository(string filePath)
    {
        _filePath = filePath;
        _settingsService = new SettingsService();
    }

    public string GetFavoriteTeam()
    {
        if (!_settingsService.SettingsExist(_filePath))
            return string.Empty;
        return _settingsService.Load(_filePath).FavoriteTeamFifaCode;
    }

    public void SaveFavoriteTeam(string fifaCode)
    {
        var settings = _settingsService.SettingsExist(_filePath)
            ? _settingsService.Load(_filePath)
            : new AppSettings();
        settings.FavoriteTeamFifaCode = fifaCode;
        _settingsService.Save(_filePath, settings);
    }

    public List<string> GetFavoritePlayers()
    {
        if (!_settingsService.SettingsExist(_filePath))
            return new List<string>();
        return _settingsService.Load(_filePath).FavoritePlayerNames;
    }

    public void SaveFavoritePlayers(List<string> playerNames)
    {
        var settings = _settingsService.SettingsExist(_filePath)
            ? _settingsService.Load(_filePath)
            : new AppSettings();
        settings.FavoritePlayerNames = playerNames.Take(3).ToList();
        _settingsService.Save(_filePath, settings);
    }
}
