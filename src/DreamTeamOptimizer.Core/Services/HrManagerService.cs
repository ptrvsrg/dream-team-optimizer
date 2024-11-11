using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Core.Interfaces.IServices;

namespace DreamTeamOptimizer.Core.Services;

public class HrManagerService(IStrategy strategy) : IHrManagerService
{
    public List<Team> BuildTeams(List<Employee> teamLeads, List<Employee> juniors,
        List<WishList> teamLeadsWishlists, List<WishList> juniorsWishlists)
    {
        return strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();
    }
}