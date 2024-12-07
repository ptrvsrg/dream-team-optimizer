using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.Core.Interfaces.Repositories;

namespace DreamTeamOptimizer.Core.Interfaces.Repositories;

public interface IHackathonRepository : IGenericRepository<Hackathon>
{
    double FindAverageResult();
}