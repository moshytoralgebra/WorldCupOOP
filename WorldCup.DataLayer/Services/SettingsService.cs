using WorldCup.DataLayer.Models;

namespace WorldCup.DataLayer.Services;

public class SettingsService
{
    public bool SettingsExist(string filePath) => File.Exists(filePath);

    public AppSettings Load(string filePath)
    {
        try
        {
            var settings = new AppSettings();
            foreach (var line in File.ReadAllLines(filePath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                    continue;

                var idx = trimmed.IndexOf('=');
                if (idx < 0)
                    throw new SettingsException($"Invalid settings line (no '='): {trimmed}");

                var key = trimmed[..idx].Trim();
                var value = trimmed[(idx + 1)..].Trim();

                switch (key)
                {
                    case "gender":
                        settings.Gender = value;
                        break;
                    case "language":
                        settings.Language = value;
                        break;
                    case "favorite_team":
                        settings.FavoriteTeamFifaCode = value;
                        break;
                    case "favorite_players":
                        settings.FavoritePlayerNames = string.IsNullOrWhiteSpace(value)
                            ? new List<string>()
                            : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(p => p.Trim())
                                   .ToList();
                        break;
                }
            }
            return settings;
        }
        catch (SettingsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SettingsException("Failed to load settings file.", ex);
        }
    }

    public void Save(string filePath, AppSettings settings)
    {
        try
        {
            var players = settings.FavoritePlayerNames.Count > 0
                ? string.Join(",", settings.FavoritePlayerNames)
                : string.Empty;

            var lines = new[]
            {
                $"gender={settings.Gender}",
                $"language={settings.Language}",
                $"favorite_team={settings.FavoriteTeamFifaCode}",
                $"favorite_players={players}"
            };
            File.WriteAllLines(filePath, lines);
        }
        catch (Exception ex)
        {
            throw new SettingsException("Failed to save settings file.", ex);
        }
    }
}
