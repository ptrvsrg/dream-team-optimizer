using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.ConsoleApp.Services.Mappers;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Core.Models;
using Serilog;

namespace DreamTeamOptimizer.ConsoleApp.Services;

public class HrManagerService(IStrategy strategy, ITeamRepository teamRepository) : IHrManagerService
{
    public List<Team> BuildTeams(List<Employee> teamLeads, List<Employee> juniors, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists, int hackathonId)
    {
        Log.Information("build teams");

        var teamModels = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();

        var teams = TeamMapper.ToEntities(teamModels);
        teams.ForEach(t => t.HackathonId = hackathonId);
        
        teamRepository.CreateAll(teams);

        return teamModels;
    }
}