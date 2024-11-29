using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.Core.Persistence.Repositories;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class TeamRepository: GenericRepository<Team>, ITeamRepository
{
    public TeamRepository(AppDbContext dbContext): base(dbContext)
    {
    }
}