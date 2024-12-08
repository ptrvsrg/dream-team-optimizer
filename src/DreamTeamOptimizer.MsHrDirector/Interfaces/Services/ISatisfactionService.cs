using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrDirector.Interfaces.Services;

public interface ISatisfactionService
{
    List<Satisfaction> Evaluate(List<Team> teams, List<WishList> teamLeadsWishlists, List<WishList> juniorsWishlists);
}