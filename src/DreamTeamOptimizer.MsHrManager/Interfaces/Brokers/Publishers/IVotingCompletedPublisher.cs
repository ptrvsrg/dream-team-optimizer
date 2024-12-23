using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;

public interface IVotingCompletedPublisher
{
    void CompleteVoting(List<Employee> teamLeads, List<Employee> juniors, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlist, int hackathonId);
}