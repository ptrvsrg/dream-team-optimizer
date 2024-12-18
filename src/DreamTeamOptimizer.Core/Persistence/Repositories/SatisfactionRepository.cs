using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence.Entities;

namespace DreamTeamOptimizer.Core.Persistence.Repositories;

public class SatisfactionRepository : GenericRepository<Satisfaction>, ISatisfactionRepository
{
    public SatisfactionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}