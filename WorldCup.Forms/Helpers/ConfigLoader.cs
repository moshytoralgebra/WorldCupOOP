using WorldCup.DataLayer.Services;

namespace WorldCup.Forms.Helpers;

public static class ConfigLoader
{
    public static (string mode, string jsonBasePath) Load()
    {
        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "dataconfig.txt");

        if (!File.Exists(configPath))
            throw new DataServiceException($"Configuration file not found: {configPath}");

        string mode = string.Empty;
        string jsonBasePath = string.Empty;

        try
        {
            foreach (var line in File.ReadAllLines(configPath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                    continue;

                var idx = trimmed.IndexOf('=');
                if (idx < 0) continue;

                var key = trimmed[..idx].Trim();
                var value = trimmed[(idx + 1)..].Trim();

                switch (key)
                {
                    case "mode": mode = value; break;
                    case "json_base_path": jsonBasePath = value; break;
                }
            }
        }
        catch (Exception ex) when (ex is not DataServiceException)
        {
            throw new DataServiceException("Failed to read the data configuration file.", ex);
        }

        if (string.IsNullOrEmpty(mode))
            throw new DataServiceException("Configuration file is missing the required 'mode' key.");

        if (!string.IsNullOrEmpty(jsonBasePath))
            jsonBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jsonBasePath);

        return (mode, jsonBasePath);
    }
}
