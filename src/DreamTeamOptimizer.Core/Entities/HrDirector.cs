using DreamTeamOptimizer.Core.Exceptions;

namespace DreamTeamOptimizer.Core.Entities;

public class HrDirector
{
    public double CalculateDistributionHarmony(IEnumerable<Team> teams, IEnumerable<WishList> teamLeadsWishlists,
        IEnumerable<WishList> juniorsWishlists)
    {
        if (teams.Count() == 0) throw new NoTeamsException();

        var teamLeadsWishlistsDict =
            teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());

        var satisfactions = new List<double>();
        foreach (var team in teams)
        {
            satisfactions.Add(CalculateSatisfaction(team.TeamLead.Id, team.Junior.Id, teamLeadsWishlistsDict));
            satisfactions.Add(CalculateSatisfaction(team.Junior.Id, team.TeamLead.Id, juniorsWishlistsDict));
        }

        return Helpers.Math.CalculateHarmonicMean(satisfactions);
    }

    private double CalculateSatisfaction(int employeeId, int selectedEmployeeId, Dictionary<int, List<int>> wishlists)
    {
        if (!wishlists.TryGetValue(employeeId, out var wishlist)) throw new WishListNotFoundException(employeeId);

        var index = wishlist.IndexOf(selectedEmployeeId);
        if (index == -1) throw new EmployeeInWishListNotFoundException(employeeId, selectedEmployeeId);
        return wishlist.Count - index;
    }
}