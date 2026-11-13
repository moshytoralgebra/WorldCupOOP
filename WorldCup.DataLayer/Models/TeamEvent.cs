namespace WorldCup.DataLayer.Models;

public class TeamEvent
{
    public int Id { get; set; }
    public string TypeOfEvent { get; set; } = string.Empty;
    public string Player { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
}
