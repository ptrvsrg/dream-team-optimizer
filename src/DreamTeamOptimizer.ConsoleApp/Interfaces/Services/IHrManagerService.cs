using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.ConsoleApp.Interfaces.Services;

public interface IHrManagerService
{
    List<Team> BuildTeams(List<Employee> teamLeads, List<Employee> juniors, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists, int hackathonId);
}