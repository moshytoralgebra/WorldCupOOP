namespace WorldCup.DataLayer.Models;

public class AppSettings
{
    public string Gender { get; set; } = "men";
    public string Language { get; set; } = "en";
    public string FavoriteTeamFifaCode { get; set; } = string.Empty;
    public List<string> FavoritePlayerNames { get; set; } = new();
}
