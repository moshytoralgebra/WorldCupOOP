using WorldCup.DataLayer.Models;

namespace WorldCup.DataLayer.Services;

public interface IDataService
{
    Task<List<Team>> GetTeamsAsync(string gender);

    Task<List<Match>> GetMatchesAsync(string gender);

    Task<List<Match>> GetMatchesByTeamAsync(string gender, string fifaCode);
}
