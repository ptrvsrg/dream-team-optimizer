using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Persistence;
using DreamTeamOptimizer.ConsoleApp.Services.Mappers;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Core.Models;
using Serilog;

namespace DreamTeamOptimizer.ConsoleApp.Services;

public class HrManagerService(IStrategy strategy, ITeamRepositroy teamRepository) : IHrManagerService
{
    public List<Team> BuildTeams(List<Employee> teamLeads, List<Employee> juniors, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists, int hackathonId)
    {
        Log.Information("build teams");

        var teamModels = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();

        var teams = TeamMapper.ToEntities(teamModels);
        teams.ForEach(t => t.HackathonId = hackathonId);
        
        teamRepository.InsertAll(teams);
        teamRepository.Save();

        return teamModels;
    }
}