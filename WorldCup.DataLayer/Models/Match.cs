namespace WorldCup.DataLayer.Models;

public class Match
{
    public string Venue { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string FifaId { get; set; } = string.Empty;
    public string Attendance { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public string HomeTeamCountry { get; set; } = string.Empty;
    public string AwayTeamCountry { get; set; } = string.Empty;
    public string DateTime { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
    public string WinnerCode { get; set; } = string.Empty;
    public TeamStatistics HomeTeam { get; set; } = new();
    public TeamStatistics AwayTeam { get; set; } = new();
}
