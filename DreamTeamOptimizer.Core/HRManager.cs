namespace DreamTeamOptimizer.Core;

public class HRManager
{
    public IStrategy Strategy { get; }

    public HRManager(IStrategy strategy)
    {
        Strategy = strategy;
    }

    public IEnumerable<WishList> VoteEmployees(IEnumerable<Employee> employees, IEnumerable<Employee> variants)
    {
        return employees.Select(employee => employee.GetWishlist(variants)).ToList();
    }

    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists)
    {
        return Strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
    }
}