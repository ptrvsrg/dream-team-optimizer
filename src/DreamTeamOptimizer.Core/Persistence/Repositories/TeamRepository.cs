using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence.Entities;

namespace DreamTeamOptimizer.Core.Persistence.Repositories;

public class TeamRepository : GenericRepository<Team>, ITeamRepository
{
    public TeamRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}