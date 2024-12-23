namespace DreamTeamOptimizer.Core.Models.Events;

public record HackathonResultEvent(
    int Id,
    List<WishList> JuniorsWishlists,
    List<WishList> TeamLeadsWishlists,
    List<Team> Teams);