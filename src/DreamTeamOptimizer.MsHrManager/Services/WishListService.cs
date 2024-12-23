using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using DreamTeamOptimizer.MsHrManager.Models.Events;
using DreamTeamOptimizer.MsHrManager.Models.Sessions;
using Microsoft.Extensions.Caching.Memory;
using Employee = DreamTeamOptimizer.Core.Models.Employee;
using WishList = DreamTeamOptimizer.Core.Models.WishList;

namespace DreamTeamOptimizer.MsHrManager.Services;

public class WishListService(
    ILogger<WishListService> logger,
    IMemoryCache cache,
    IVotingStartedPublisher votingStartedPublisher,
    IVotingCompletedPublisher votingCompletedPublisher) : IWishListService
{
    private TimeSpan CacheDuration => TimeSpan.FromMinutes(1);

    public void StartVoting(List<Employee> teamLeads, List<Employee> juniors, int hackathonId)
    {
        logger.LogInformation("vote employees");

        logger.LogDebug("create session");

        var session = new VotingSession(0, teamLeads.Count + juniors.Count, teamLeads, juniors, new List<WishList>(),
            new List<WishList>());
        cache.Set(hackathonId, session, CacheDuration);

        var voting = new VotingStartedEvent(hackathonId, teamLeads, juniors);
        votingStartedPublisher.StartVoting(voting);
    }

    public void SaveWishListToSession(WishList wishList, int hackathonId)
    {
        logger.LogInformation("save wishlists");

        logger.LogDebug("update session");
        var session = cache.Get<VotingSession>(hackathonId);
        if (session == null) throw new KeyNotFoundException("Session not found");

        if (session.Juniors.Find(j => j.Id == wishList.EmployeeId) != null)
        {
            session.JuniorsWishlists.Add(wishList);
        }
        else if (session.TeamLeads.Find(t => t.Id == wishList.EmployeeId) != null)
        {
            session.TeamLeadsWishlists.Add(wishList);
        }
        else
        {
            logger.LogWarning("unknown employee");
            return;
        }

        session.Current++;

        logger.LogDebug("save session to cache");
        cache.Set(hackathonId, session);

        if (session.Current == session.Total)
        {
            logger.LogInformation("publish voting completed event");
            votingCompletedPublisher.CompleteVoting(session.TeamLeads, session.Juniors, session.TeamLeadsWishlists,
                session.JuniorsWishlists, hackathonId);
        }
    }
}