using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrDirector.Interfaces.Services;

public interface IHackathonService
{
    HackathonSimple Create();

    void SaveResult(List<Team> teams, List<WishList> teamLeadsWishlists, List<WishList> juniorsWishlists,
        int hackathonId);

    Hackathon GetById(int id);

    AverageHarmonicity CalculateAverageHarmonicity();
}