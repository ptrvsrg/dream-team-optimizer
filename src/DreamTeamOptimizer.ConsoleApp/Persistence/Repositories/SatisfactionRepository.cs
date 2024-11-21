using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class SatisfactionRepository : GenericRepository<Satisfaction>, ISatisfactionRepository
{
    public SatisfactionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}