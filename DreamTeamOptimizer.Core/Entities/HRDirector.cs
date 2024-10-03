using DreamTeamOptimizer.Core.Exceptions;

namespace DreamTeamOptimizer.Core.Entities;

public class HRDirector
{
    public double CalculateDistributionHarmony(IEnumerable<Team> teams, IEnumerable<WishList> teamLeadsWishlists,
        IEnumerable<WishList> juniorsWishlists)
    {
        var teamLeadsWishlistsDict =
            teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());

        var satisfactions = new List<double>();
        foreach (var team in teams)
        {
            satisfactions.Add(CalculateSatisfaction(team.TeamLead.Id, team.Junior.Id, teamLeadsWishlistsDict));
            satisfactions.Add(CalculateSatisfaction(team.Junior.Id, team.TeamLead.Id, juniorsWishlistsDict));
        }

        return CalculateHarmonicMean(satisfactions);
    }

    private double CalculateSatisfaction(int employeeId, int selectedEmployeeId, Dictionary<int, List<int>> wishlists)
    {
        if (!wishlists.TryGetValue(employeeId, out var wishlist)) throw new WishListNotFoundException(employeeId);

        var index = wishlist.IndexOf(selectedEmployeeId);
        if (index == -1) throw new EmployeeInWishListNotFoundException(employeeId, selectedEmployeeId);
        return wishlist.Count - index;
    }

    private double CalculateHarmonicMean(List<double> values)
    {
        var sum = 0.0;
        foreach (var value in values)
        {
            if (value == 0) throw new DivideByZeroException();
            sum += 1.0 / value;
        }

        return values.Count / sum;
    }
}