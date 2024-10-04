using DreamTeamOptimizer.Core.Interfaces;

namespace DreamTeamOptimizer.Core.Entities;

public class HrManager
{
    private readonly IStrategy _strategy;

    public HrManager(IStrategy strategy)
    {
        _strategy = strategy;
    }

    public List<WishList> VoteEmployees(IEnumerable<Employee> employees, IEnumerable<Employee> variants)
    {
        return employees.Select(employee => employee.GetWishlist(variants)).ToList();
    }

    public List<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists)
    {
        return _strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList();
    }
}