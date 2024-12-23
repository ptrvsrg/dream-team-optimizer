using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Models.Events;

public record VotingCompletedEvent(
    int HackathonId,
    List<Employee> TeamLeads,
    List<Employee> Juniors,
    List<WishList> TeamLeadsWishlists,
    List<WishList> JuniorsWishlists);