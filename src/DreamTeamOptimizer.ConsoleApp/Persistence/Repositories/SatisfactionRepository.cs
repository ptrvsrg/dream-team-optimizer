using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.Core.Persistence.Repositories;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class SatisfactionRepository : GenericRepository<Satisfaction>, ISatisfactionRepository
{
    public SatisfactionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}