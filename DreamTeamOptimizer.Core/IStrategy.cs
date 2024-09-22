namespace DreamTeamOptimizer.Core;

public interface IStrategy
{
    IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists);
}