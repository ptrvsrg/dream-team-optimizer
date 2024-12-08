namespace DreamTeamOptimizer.Core.Models;

public record HackathonResult(
    List<WishList> JuniorsWishlists,
    List<WishList> TeamLeadsWishlists,
    List<Team> Teams);