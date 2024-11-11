using DreamTeamOptimizer.Core.Entities;

namespace DreamTeamOptimizer.Core.Interfaces.IServices;

public interface IHrManagerService
{
    List<Team> BuildTeams(List<Employee> teamLeads, List<Employee> juniors, 
        List<WishList> teamLeadsWishlists, List<WishList> juniorsWishlists);
}