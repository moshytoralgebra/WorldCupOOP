namespace WorldCup.DataLayer.Services;

public static class DataServiceFactory
{
    public static IDataService Create(string mode, string jsonBasePath) =>
        mode.ToLowerInvariant() switch
        {
            "api"  => new ApiDataService(),
            "file" => new JsonFileDataService(jsonBasePath),
            _      => throw new ArgumentException($"Unknown data service mode: '{mode}'", nameof(mode))
        };
}
