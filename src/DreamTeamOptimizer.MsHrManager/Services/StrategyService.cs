using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using DreamTeamOptimizer.MsHrManager.Services.Mappers;

namespace DreamTeamOptimizer.MsHrManager.Services;

public class StrategyService(ILogger<StrategyService> logger, IStrategy strategy, ITeamRepository teamRepository)
    : IStrategyService
{
    public List<Team> BuildTeams(List<Employee> teamLeads, List<Employee> juniors, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists, int hackathonId)
    {
        logger.LogInformation("build teams");

        var teamModels = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();

        var teams = TeamMapper.ToEntities(teamModels);
        teams.ForEach(t => t.HackathonId = hackathonId);

        teamRepository.CreateAll(teams);

        return teamModels;
    }
}