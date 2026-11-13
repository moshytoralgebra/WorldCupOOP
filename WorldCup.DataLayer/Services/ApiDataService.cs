using System.Text.Json;
using WorldCup.DataLayer.Models;
using WorldCup.DataLayer.Services.Dto;

namespace WorldCup.DataLayer.Services;

public class ApiDataService : IDataService
{
    private const string BaseUrl = "http://worldcup-vua.nullbit.hr";
    private static readonly HttpClient _http = new();

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<Team>> GetTeamsAsync(string gender)
    {
        try
        {
            var response = await _http.GetAsync($"{BaseUrl}/{gender}/teams/results");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Team>>(json, _jsonOptions)
                   ?? new List<Team>();
        }
        catch (HttpRequestException ex)
        {
            throw new DataServiceException("Failed to connect to the data API.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new DataServiceException("The request to the data API timed out.", ex);
        }
        catch (JsonException ex)
        {
            throw new DataServiceException("The API returned an unexpected data format.", ex);
        }
    }

    public async Task<List<Match>> GetMatchesAsync(string gender)
    {
        try
        {
            var response = await _http.GetAsync($"{BaseUrl}/{gender}/matches");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var dtos = JsonSerializer.Deserialize<List<MatchJsonDto>>(json, _jsonOptions)
                       ?? new List<MatchJsonDto>();
            return dtos.Select(MapMatch).ToList();
        }
        catch (HttpRequestException ex)
        {
            throw new DataServiceException("Failed to connect to the data API.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new DataServiceException("The request to the data API timed out.", ex);
        }
        catch (JsonException ex)
        {
            throw new DataServiceException("The API returned an unexpected data format.", ex);
        }
    }

    public async Task<List<Match>> GetMatchesByTeamAsync(string gender, string fifaCode)
    {
        try
        {
            var response = await _http.GetAsync($"{BaseUrl}/{gender}/matches/country?fifa_code={fifaCode}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var dtos = JsonSerializer.Deserialize<List<MatchJsonDto>>(json, _jsonOptions)
                       ?? new List<MatchJsonDto>();
            return dtos.Select(MapMatch).ToList();
        }
        catch (HttpRequestException ex)
        {
            throw new DataServiceException("Failed to connect to the data API.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new DataServiceException("The request to the data API timed out.", ex);
        }
        catch (JsonException ex)
        {
            throw new DataServiceException("The API returned an unexpected data format.", ex);
        }
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
