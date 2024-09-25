using DreamTeamOptimizer.Core.Interfaces;

namespace DreamTeamOptimizer.Core.Entities;

public class HRManager
{
    private IStrategy Strategy { get; }

    public HRManager(IStrategy strategy)
    {
        Strategy = strategy;
    }

    public List<WishList> VoteEmployees(IEnumerable<Employee> employees, IEnumerable<Employee> variants)
    {
        return employees.Select(employee => employee.GetWishlist(variants)).ToList();
    }

    public List<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists)
    {
        return Strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();
    }
}