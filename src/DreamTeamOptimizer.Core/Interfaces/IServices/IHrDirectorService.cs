using DreamTeamOptimizer.Core.Entities;

namespace DreamTeamOptimizer.Core.Interfaces.IServices;

public interface IHrDirectorService
{
    double CalculateDistributionHarmony(List<Team> teams, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists);
}