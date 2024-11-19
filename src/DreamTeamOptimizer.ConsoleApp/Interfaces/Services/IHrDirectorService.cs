using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.ConsoleApp.Interfaces.Services;

public interface IHrDirectorService
{
    List<Satisfaction> CalculateSatisfactions(List<Team> teams, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists, int hackathonId);
}