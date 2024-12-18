using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Interfaces;

namespace DreamTeamOptimizer.Strategies;

public class GaleShapleyStrategy : IStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists)
    {
        // Dictionaries for quick search of employee
        var teamLeadsDict = teamLeads.ToDictionary(tl => tl.Id);
        var juniorsDict = juniors.ToDictionary(j => j.Id);

        var unmatchedJuniors = new Queue<Employee>(juniors);
        var currentMatches = new Dictionary<int, int>(teamLeadsDict.Count);

        // Dictionaries for quick search of preferences
        var teamLeadsWishlistsDict = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees);

        while (unmatchedJuniors.Count > 0)
        {
            var junior = unmatchedJuniors.Dequeue();
            var juniorWishlist = juniorsWishlistsDict[junior.Id];

            foreach (var preferredTeamLeadId in juniorWishlist)
            {
                if (!teamLeadsDict.TryGetValue(preferredTeamLeadId, out var teamLead)) continue;
                var teamLeadWishlist = teamLeadsWishlistsDict[teamLead.Id];

                if (currentMatches.TryGetValue(teamLead.Id, out var currentJuniorId))
                {
                    var currentJuniorRank = Array.IndexOf(teamLeadWishlist, currentJuniorId);
                    var newJuniorRank = Array.IndexOf(teamLeadWishlist, junior.Id);

                    if (newJuniorRank >= 0 && newJuniorRank < currentJuniorRank)
                    {
                        unmatchedJuniors.Enqueue(juniorsDict[currentJuniorId]);
                        currentMatches[teamLead.Id] = junior.Id;
                        break;
                    }
                }
                else
                {
                    currentMatches[teamLead.Id] = junior.Id;
                    break;
                }
            }
        }

        // Build teams
        var teams = currentMatches.Select(match =>
        {
            var teamLead = teamLeadsDict[match.Key];
            var junior = juniorsDict[match.Value];
            return new Team(teamLead.Id, junior.Id);
        }).ToList();

        return teams;
    }
}