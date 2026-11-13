using System.Text.Json;
using WorldCup.DataLayer.Models;
using WorldCup.DataLayer.Services.Dto;

namespace WorldCup.DataLayer.Services;

public class JsonFileDataService : IDataService
{
    private readonly string _jsonBasePath;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public JsonFileDataService(string jsonBasePath)
    {
        _jsonBasePath = jsonBasePath;
    }

    public Task<List<Team>> GetTeamsAsync(string gender)
    {
        try
        {
            var path = Path.Combine(_jsonBasePath, gender, "results.json");
            var json = File.ReadAllText(path);
            var teams = JsonSerializer.Deserialize<List<Team>>(json, _jsonOptions)
                        ?? new List<Team>();
            return Task.FromResult(teams);
        }
        catch (FileNotFoundException ex)
        {
            throw new DataServiceException($"Data file not found: {ex.FileName}", ex);
        }
        catch (JsonException ex)
        {
            throw new DataServiceException("The data file contains an unexpected format.", ex);
        }
    }

    public Task<List<Match>> GetMatchesAsync(string gender)
    {
        try
        {
            var path = Path.Combine(_jsonBasePath, gender, "matches.json");
            var json = File.ReadAllText(path);
            var dtos = JsonSerializer.Deserialize<List<MatchJsonDto>>(json, _jsonOptions)
                       ?? new List<MatchJsonDto>();
            var matches = dtos.Select(MapMatch).ToList();
            return Task.FromResult(matches);
        }
        catch (FileNotFoundException ex)
        {
            throw new DataServiceException($"Data file not found: {ex.FileName}", ex);
        }
        catch (JsonException ex)
        {
            throw new DataServiceException("The data file contains an unexpected format.", ex);
        }
    }

    public Task<List<Match>> GetMatchesByTeamAsync(string gender, string fifaCode)
    {
        var allMatches = GetMatchesAsync(gender).Result;
        var filtered = allMatches
            .Where(m => m.HomeTeam.Code == fifaCode || m.AwayTeam.Code == fifaCode)
            .ToList();
        return Task.FromResult(filtered);
    }

    private static Match MapMatch(MatchJsonDto dto) => new()
    {
        Venue = dto.Venue,
        Location = dto.Location,
        Status = dto.Status,
        FifaId = dto.FifaId,
        Attendance = dto.Attendance,
        StageName = dto.StageName,
        HomeTeamCountry = dto.HomeTeamCountry,
        AwayTeamCountry = dto.AwayTeamCountry,
        DateTime = dto.DateTime,
        Winner = dto.Winner,
        WinnerCode = dto.WinnerCode,
        HomeTeam = MapTeamStatistics(dto.HomeTeam, dto.HomeTeamStatistics, dto.HomeTeamEvents),
        AwayTeam = MapTeamStatistics(dto.AwayTeam, dto.AwayTeamStatistics, dto.AwayTeamEvents)
    };

    private static TeamStatistics MapTeamStatistics(
        TeamScoreDto score, TeamStatsDto stats, List<TeamEventDto> events) => new()
    {
        Country = score.Country,
        Code = score.Code,
        Goals = score.Goals,
        Penalties = score.Penalties ?? 0,
        StartingEleven = stats.StartingEleven.Select(MapPlayer).ToList(),
        Substitutes = stats.Substitutes.Select(MapPlayer).ToList(),
        Events = events.Select(MapEvent).ToList()
    };

    private static Player MapPlayer(PlayerDto dto) => new()
    {
        Name = dto.Name,
        Captain = dto.Captain,
        ShirtNumber = dto.ShirtNumber,
        Position = dto.Position
    };

    private static TeamEvent MapEvent(TeamEventDto dto) => new()
    {
        Id = dto.Id,
        TypeOfEvent = dto.TypeOfEvent,
        Player = dto.Player,
        Time = dto.Time
    };
}
