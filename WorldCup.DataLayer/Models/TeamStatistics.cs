namespace WorldCup.DataLayer.Models;

public class TeamStatistics
{
    public string Country { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Goals { get; set; }
    public int Penalties { get; set; }
    public List<Player> StartingEleven { get; set; } = new();
    public List<Player> Substitutes { get; set; } = new();
    public List<TeamEvent> Events { get; set; } = new();
}
