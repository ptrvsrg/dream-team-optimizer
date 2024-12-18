using DreamTeamOptimizer.Core.Persistence.Entities;

namespace DreamTeamOptimizer.Core.Interfaces.Repositories;

public interface IHackathonRepository : IGenericRepository<Hackathon>
{
    double FindAverageResult();
}