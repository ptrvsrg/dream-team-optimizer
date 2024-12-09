using Nsu.HackathonProblem.Contracts;

namespace DreamTeamOptimizer.Core.Interfaces.IServices;

public interface IHrManagerService
{
    List<Team> BuildTeams(List<Employee> teamLeads, List<Employee> juniors, 
        List<Wishlist> teamLeadsWishlists, List<Wishlist> juniorsWishlists);
}