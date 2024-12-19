using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Interfaces;

namespace DreamTeamOptimizer.Strategies;

public class BipartiteGraphStrategy : IStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists)
    {
        var teams = new List<Team>(teamLeads.Count());
        var edges = new List<Edge>(teamLeads.Count() * juniors.Count());

        // Dictionaries for quick search of preferences
        var teamLeadsWishlistsDict = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);

        // Build a graph with calculation of edge weights
        foreach (var junior in juniors)
        {
            if (!juniorsWishlistsDict.ContainsKey(junior.Id)) continue; // Skip it if there is no wishlist

            foreach (var teamLead in teamLeads)
            {
                if (!teamLeadsWishlistsDict.ContainsKey(teamLead.Id)) continue; // Skip it if there is no wishlist

                var juniorWishList = juniorsWishlistsDict[junior.Id];
                var teamLeadWishList = teamLeadsWishlistsDict[teamLead.Id];

                // Get preference indexes
                var juniorPreferenceIndex = Array.IndexOf(juniorWishList, teamLead.Id);
                var teamLeadPreferenceIndex = Array.IndexOf(teamLeadWishList, junior.Id);

                // If it is not in the list of preferences, skip it
                if (juniorPreferenceIndex == -1 || teamLeadPreferenceIndex == -1) continue;

                // Calculating satisfaction (sorted by decrease)
                var juniorSatisfaction = juniorWishList.Length - juniorPreferenceIndex;
                var teamLeadSatisfaction = teamLeadWishList.Length - teamLeadPreferenceIndex;
                var weight = juniorSatisfaction * teamLeadSatisfaction;

                // Add edge
                edges.Add(new Edge(junior, teamLead, weight));
            }
        }

        // Sorting the edges in descending order of weight
        var sortedEdges = edges.OrderByDescending(e => e.Weight);

        // Build teams
        var matchedJuniors = new HashSet<int>();
        var matchedTeamLeads = new HashSet<int>();

        foreach (var edge in sortedEdges)
            if (!matchedJuniors.Contains(edge.Junior.Id) && !matchedTeamLeads.Contains(edge.TeamLead.Id))
            {
                teams.Add(new Team(edge.TeamLead.Id, edge.Junior.Id));
                matchedJuniors.Add(edge.Junior.Id);
                matchedTeamLeads.Add(edge.TeamLead.Id);
            }

        return teams;
    }

    private class Edge
    {
        public Employee Junior { get; }
        public Employee TeamLead { get; }
        public double Weight { get; }

        public Edge(Employee junior, Employee teamLead, double weight)
        {
            Junior = junior;
            TeamLead = teamLead;
            Weight = weight;
        }
    }
}