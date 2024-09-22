using DreamTeamOptimizer.Core;

namespace DreamTeamOptimizer.Strategies;

public class BipartiteGraphWithRatingStrategy : IStrategy
{
    private const double
        RatingCoefficient = 0.3;

    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists)
    {
        var teams = new List<Team>();
        var edges = new List<Edge>();

        // Convert to wishlist list to dictionary
        var teamLeadsWishlistsDict = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());

        // Calculation of ratings of juniors and team leads
        var juniorRatings = CalculateRatings(juniors, teamLeadsWishlistsDict);
        var teamLeadRatings = CalculateRatings(teamLeads, juniorsWishlistsDict);

        // Construction of a complete bipartite graph with calculation of edge weights
        foreach (var junior in juniors)
        {
            foreach (var teamLead in teamLeads)
            {
                var juniorPreferenceIndex = juniorsWishlistsDict[junior.Id].IndexOf(teamLead.Id);
                var teamLeadPreferenceIndex = teamLeadsWishlistsDict[teamLead.Id].IndexOf(junior.Id);

                // Calculate satisfaction
                int juniorSatisfaction = juniorsWishlistsDict[junior.Id].Count - juniorPreferenceIndex;
                int teamLeadSatisfaction = teamLeadsWishlistsDict[teamLead.Id].Count - teamLeadPreferenceIndex;
                int baseWeight = juniorSatisfaction + teamLeadSatisfaction;

                // Adjust the weight taking into account the ratings
                double juniorRating = juniorRatings[junior.Id] * RatingCoefficient;
                double teamLeadRating = teamLeadRatings[teamLead.Id] * RatingCoefficient;
                double adjustedWeight = baseWeight + juniorRating + teamLeadRating;

                edges.Add(new Edge(junior, teamLead, adjustedWeight));
            }
        }

        // Sort the edges in descending order of weight
        var sortedEdges = edges.OrderByDescending(e => e.Weight).ToList();

        // Формирование команд
        var matchedJuniors = new HashSet<int>();
        var matchedTeamLeads = new HashSet<int>();
        foreach (var edge in sortedEdges)
        {
            if (!matchedJuniors.Contains(edge.Junior.Id) && !matchedTeamLeads.Contains(edge.TeamLead.Id))
            {
                teams.Add(new Team(edge.TeamLead, edge.Junior));

                // Mark the junior and team lead as distributed
                matchedJuniors.Add(edge.Junior.Id);
                matchedTeamLeads.Add(edge.TeamLead.Id);
            }
        }

        return teams;
    }

    // Function for calculating employee ratings (juniors or team leads)
    private Dictionary<int, double> CalculateRatings(IEnumerable<Employee> employees,
        Dictionary<int, List<int>> wishlists)
    {
        var popularityScores = new Dictionary<int, int>();

        // Calculate the sum of places for each employee in other people's preference lists
        foreach (var wishlist in wishlists.Values)
        {
            for (int i = 0; i < wishlist.Count; i++)
            {
                if (!popularityScores.ContainsKey(wishlist[i]))
                {
                    popularityScores[wishlist[i]] = 0;
                }

                popularityScores[wishlist[i]] += i + 1; // The lower the place, the better (so we add index + 1)
            }
        }

        // Sort employees by their total places (the lower the total, the more popular)
        var rankedEmployees = popularityScores.OrderBy(p => p.Value).ToList();

        // Assign ratings: the higher the place in the list, the higher the rating
        var ratings = new Dictionary<int, double>();
        int totalEmployees = employees.Count();
        for (int i = 0; i < rankedEmployees.Count; i++)
        {
            var employeeId = rankedEmployees[i].Key;
            // The closer to the beginning of the list, the higher the rating: totalEmployees - place in the list
            ratings[employeeId] = totalEmployees - i;
        }

        return ratings;
    }

    // Class of an edge that connects a junior and a team lead with a certain weight
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
