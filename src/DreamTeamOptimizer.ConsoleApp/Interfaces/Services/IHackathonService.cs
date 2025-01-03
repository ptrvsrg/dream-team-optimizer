using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.ConsoleApp.Interfaces.Services;

public interface IHackathonService
{
    Hackathon Conduct();

    double CalculateAverageHarmonicity();
    
    Hackathon? FindById(int id);
}