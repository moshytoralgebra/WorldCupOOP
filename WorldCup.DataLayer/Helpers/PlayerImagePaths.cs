namespace WorldCup.DataLayer.Helpers;

public static class PlayerImagePaths
{
    public static string GetFolder() =>
        Directory.CreateDirectory(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WorldCupApp", "PlayerImages")).FullName;

    public static string GetImagePath(string fifaCode, int shirtNumber) =>
        Path.Combine(GetFolder(), $"{fifaCode}_{shirtNumber}.png");
}
