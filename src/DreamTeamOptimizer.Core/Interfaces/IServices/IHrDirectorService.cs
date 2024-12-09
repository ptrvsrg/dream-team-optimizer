using Nsu.HackathonProblem.Contracts;

namespace DreamTeamOptimizer.Core.Interfaces.IServices;

public interface IHrDirectorService
{
    double CalculateDistributionHarmony(List<Team> teams, List<Wishlist> teamLeadsWishlists,
        List<Wishlist> juniorsWishlists);
}