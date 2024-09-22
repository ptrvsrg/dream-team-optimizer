namespace DreamTeamOptimizer.Core;

public class HRDirector
{
    public double CalculateDistributionHarmony(IEnumerable<Team> teams, IEnumerable<WishList> teamLeadsWishlists,
        IEnumerable<WishList> juniorsWishlists)
    {
        var teamLeadsWishlistsDict = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());
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
        if (!wishlists.ContainsKey(employeeId)) return 0;
        var wishlist = wishlists[employeeId];
        
        int index = wishlist.IndexOf(selectedEmployeeId);
        if (index == -1) return 0;
        return wishlist.Count - index;
    }

    private double CalculateHarmonicMean(IEnumerable<double> values)
    {
        double sum = 0;
        foreach (var value in values)
        {
            if (value == 0) return 0;
            sum += 1.0 / value;
        }

        return values.Count() / sum;
    }
}