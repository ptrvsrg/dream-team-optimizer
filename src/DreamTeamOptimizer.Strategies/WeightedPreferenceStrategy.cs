using Nsu.HackathonProblem.Contracts;

namespace DreamTeamOptimizer.Strategies;

public class WeightedPreferenceStrategy : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        // Dictionaries for quick search of employee
        var teamLeadsDict = teamLeads.ToDictionary(tl => tl.Id);
        var juniorsDict = juniors.ToDictionary(j => j.Id);

        var unmatchedJuniors = new Queue<Employee>(juniors); // The queue of juniors who have not yet found a pair
        var currentMatches =
            new Dictionary<int, int>(teamLeadsDict.Count); // Stores current matches: junior.Id -> teamLead.Id

        // Dictionaries for quick search of wishlists
        var teamLeadsWishlistsDict = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);

        // Array for storing preference weights
        var weights = new Dictionary<int, Dictionary<int, double>>(teamLeadsDict.Count);

        // Calculating preference weights
        foreach (var teamLead in teamLeadsDict.Values)
        {
            weights[teamLead.Id] = new Dictionary<int, double>(juniorsDict.Count);
            foreach (var junior in juniorsDict.Values)
            {
                var teamLeadPreference = GetWeightedPreference(teamLead.Id, junior.Id, teamLeadsWishlistsDict);
                var juniorPreference = GetWeightedPreference(junior.Id, teamLead.Id, juniorsWishlistsDict);
                weights[teamLead.Id][junior.Id] = teamLeadPreference + juniorPreference;
            }
        }

        while (unmatchedJuniors.Count > 0)
        {
            var junior = unmatchedJuniors.Dequeue();
            Employee bestMatch = null;
            var maxWeight = double.MinValue;

            foreach (var teamLead in teamLeadsDict.Values)
                if (!currentMatches.ContainsKey(teamLead.Id) ||
                    weights[teamLead.Id][currentMatches[teamLead.Id]] < weights[teamLead.Id][junior.Id])
                {
                    var weight = weights[teamLead.Id][junior.Id];
                    if (weight > maxWeight)
                    {
                        maxWeight = weight;
                        bestMatch = teamLead;
                    }
                }

            if (bestMatch != null)
            {
                if (currentMatches.TryGetValue(bestMatch.Id, out var replacedJuniorId))
                    unmatchedJuniors.Enqueue(juniorsDict[replacedJuniorId]);

                currentMatches[bestMatch.Id] = junior.Id;
            }
        }

        // Build teams
        return currentMatches.Select(match =>
        {
            var teamLead = teamLeadsDict[match.Key];
            var junior = juniorsDict[match.Value];
            return new Team(teamLead, junior);
        }).ToList();
    }

    // Method for calculating preference weight
    private double GetWeightedPreference(int employeeId, int targetId, Dictionary<int, int[]> wishlists)
    {
        if (wishlists.TryGetValue(employeeId, out var wishlist))
        {
            var index = Array.IndexOf(wishlist, targetId);
            if (index >= 0) return wishlist.Length - index; // The higher in the list, the greater the weight
        }

        return 0;
    }
}