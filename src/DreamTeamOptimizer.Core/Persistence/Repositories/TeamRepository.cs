using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.Core.Persistence.Repositories;

namespace DreamTeamOptimizer.Core.Persistence.Repositories;

public class TeamRepository: GenericRepository<Team>, ITeamRepository
{
    public TeamRepository(AppDbContext dbContext): base(dbContext)
    {
    }
}