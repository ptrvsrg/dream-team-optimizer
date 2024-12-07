using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.Core.Persistence.Repositories;

namespace DreamTeamOptimizer.Core.Persistence.Repositories;

public class SatisfactionRepository : GenericRepository<Satisfaction>, ISatisfactionRepository
{
    public SatisfactionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}