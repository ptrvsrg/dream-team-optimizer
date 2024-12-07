using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Services;

public interface IStrategyService
{
    List<Team> BuildTeams(List<Employee> teamLeads, List<Employee> juniors, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists, int hackathonId);
}