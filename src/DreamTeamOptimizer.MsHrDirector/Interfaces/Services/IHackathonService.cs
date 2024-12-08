using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrDirector.Interfaces.Services;

public interface IHackathonService
{
    HackathonSimple Create();

    void SaveResult(HackathonResult result, int hackathonId);

    Hackathon GetById(int id);

    AverageHarmonicity CalculateAverageHarmonicity();
}