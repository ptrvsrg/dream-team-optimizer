using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsHrManager.Models.Events;

namespace DreamTeamOptimizer.MsHrManager.Brokers.Publishers;

public class VotingCompletedPublisher(ILogger<VotingCompletedPublisher> logger, IMemoryBus bus)
    : IVotingCompletedPublisher
{
    public void CompleteVoting(List<Employee> teamLeads, List<Employee> juniors, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlist, int hackathonId)
    {
        logger.LogInformation("publish voting completed event");
        bus.Publish<VotingCompletedEvent>(new(hackathonId, teamLeads, juniors, teamLeadsWishlists, juniorsWishlist));
    }
}