using DreamTeamOptimizer.Core.Interfaces.IServices;
using Nsu.HackathonProblem.Contracts;

namespace DreamTeamOptimizer.Core.Services;

public class HrManagerService(ITeamBuildingStrategy strategy) : IHrManagerService
{
    public List<Team> BuildTeams(List<Employee> teamLeads, List<Employee> juniors,
        List<Wishlist> teamLeadsWishlists, List<Wishlist> juniorsWishlists)
    {
        return strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();
    }
}