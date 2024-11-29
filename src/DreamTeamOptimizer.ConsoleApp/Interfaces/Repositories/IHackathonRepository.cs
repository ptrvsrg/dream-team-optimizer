using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.Core.Interfaces.Repositories;

namespace DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;

public interface IHackathonRepository : IGenericRepository<Hackathon>
{
    double FindAverageResult();
}