using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.Core.Interfaces;

public interface IStrategy
{
    IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists);
}