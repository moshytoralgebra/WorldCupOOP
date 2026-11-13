using System.Text.Json.Serialization;

namespace WorldCup.DataLayer.Services.Dto;

internal class MatchJsonDto
{
    [JsonPropertyName("venue")] public string Venue { get; set; } = string.Empty;
    [JsonPropertyName("location")] public string Location { get; set; } = string.Empty;
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("fifa_id")] public string FifaId { get; set; } = string.Empty;
    [JsonPropertyName("attendance")] public string Attendance { get; set; } = string.Empty;
    [JsonPropertyName("stage_name")] public string StageName { get; set; } = string.Empty;
    [JsonPropertyName("home_team_country")] public string HomeTeamCountry { get; set; } = string.Empty;
    [JsonPropertyName("away_team_country")] public string AwayTeamCountry { get; set; } = string.Empty;
    [JsonPropertyName("datetime")] public string DateTime { get; set; } = string.Empty;
    [JsonPropertyName("winner")] public string Winner { get; set; } = string.Empty;
    [JsonPropertyName("winner_code")] public string WinnerCode { get; set; } = string.Empty;
    [JsonPropertyName("home_team")] public TeamScoreDto HomeTeam { get; set; } = new();
    [JsonPropertyName("away_team")] public TeamScoreDto AwayTeam { get; set; } = new();
    [JsonPropertyName("home_team_statistics")] public TeamStatsDto HomeTeamStatistics { get; set; } = new();
    [JsonPropertyName("away_team_statistics")] public TeamStatsDto AwayTeamStatistics { get; set; } = new();
    [JsonPropertyName("home_team_events")] public List<TeamEventDto> HomeTeamEvents { get; set; } = new();
    [JsonPropertyName("away_team_events")] public List<TeamEventDto> AwayTeamEvents { get; set; } = new();
}

internal class TeamScoreDto
{
    [JsonPropertyName("country")] public string Country { get; set; } = string.Empty;
    [JsonPropertyName("code")] public string Code { get; set; } = string.Empty;
    [JsonPropertyName("goals")] public int Goals { get; set; }
    [JsonPropertyName("penalties")] public int? Penalties { get; set; }
}

internal class TeamStatsDto
{
    [JsonPropertyName("starting_eleven")] public List<PlayerDto> StartingEleven { get; set; } = new();
    [JsonPropertyName("substitutes")] public List<PlayerDto> Substitutes { get; set; } = new();
}

internal class PlayerDto
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("captain")] public bool Captain { get; set; }
    [JsonPropertyName("shirt_number")] public int ShirtNumber { get; set; }
    [JsonPropertyName("position")] public string Position { get; set; } = string.Empty;
}

internal class TeamEventDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("type_of_event")] public string TypeOfEvent { get; set; } = string.Empty;
    [JsonPropertyName("player")] public string Player { get; set; } = string.Empty;
    [JsonPropertyName("time")] public string Time { get; set; } = string.Empty;
}
