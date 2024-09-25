using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Interfaces;

namespace DreamTeamOptimizer.Strategies;

public class GaleShapleyStrategy : IStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists)
    {
        var teamLeadsList = teamLeads.ToList();
        var juniorsList = juniors.ToList();
        
        var unmatchedJuniors = new Queue<Employee>(juniorsList);
        var currentMatches = new Dictionary<int, int>();

        var teamLeadsWishlistsDict = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());

        while (unmatchedJuniors.Count > 0)
        {
            var junior = unmatchedJuniors.Dequeue();
            var juniorWishlist = juniorsWishlistsDict[junior.Id];

            foreach (var preferredTeamLeadId in juniorWishlist)
            {
                var teamLead = teamLeadsList.First(tl => tl.Id == preferredTeamLeadId);
                var teamLeadWishlist = teamLeadsWishlistsDict[teamLead.Id];

                if (currentMatches.TryGetValue(teamLead.Id, out var currentJuniorId))
                {
                    var currentJuniorRank = teamLeadWishlist.IndexOf(currentJuniorId);
                    var newJuniorRank = teamLeadWishlist.IndexOf(junior.Id);

                    if (newJuniorRank < currentJuniorRank)
                    {
                        unmatchedJuniors.Enqueue(juniorsList.First(j => j.Id == currentJuniorId));
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

        var teams = currentMatches.Select(match =>
        {
            var teamLead = teamLeadsList.First(tl => tl.Id == match.Key);
            var junior = juniorsList.First(j => j.Id == match.Value);
            return new Team(teamLead, junior);
        }).ToList();

        return teams;
    }
}
