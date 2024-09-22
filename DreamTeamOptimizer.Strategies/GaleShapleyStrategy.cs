using DreamTeamOptimizer.Core;

namespace DreamTeamOptimizer.Strategies;

public class GaleShapleyStrategy : IStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<WishList> teamLeadsWishlists, IEnumerable<WishList> juniorsWishlists)
    {
        var unmatchedJuniors = new Queue<Employee>(juniors);
        var currentMatches = new Dictionary<int, int>();
        var proposalsMade = new Dictionary<int, HashSet<int>>();

        var teamLeadsWishlistsDict = teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());

        foreach (var teamLead in teamLeads)
        {
            proposalsMade[teamLead.Id] = new HashSet<int>();
        }

        while (unmatchedJuniors.Count > 0)
        {
            var junior = unmatchedJuniors.Dequeue();
            var juniorWishlist = juniorsWishlistsDict[junior.Id];

            foreach (var preferredTeamLeadId in juniorWishlist)
            {
                var teamLead = teamLeads.First(tl => tl.Id == preferredTeamLeadId);
                var teamLeadWishlist = teamLeadsWishlistsDict[teamLead.Id];

                if (currentMatches.ContainsKey(teamLead.Id))
                {
                    var currentJuniorId = currentMatches[teamLead.Id];
                    var currentJuniorRank = teamLeadWishlist.IndexOf(currentJuniorId);
                    var newJuniorRank = teamLeadWishlist.IndexOf(junior.Id);

                    if (newJuniorRank < currentJuniorRank)
                    {
                        unmatchedJuniors.Enqueue(juniors.First(j => j.Id == currentJuniorId));
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
            var teamLead = teamLeads.First(tl => tl.Id == match.Key);
            var junior = juniors.First(j => j.Id == match.Value);
            return new Team(teamLead, junior);
        }).ToList();

        return teams;
    }
}
