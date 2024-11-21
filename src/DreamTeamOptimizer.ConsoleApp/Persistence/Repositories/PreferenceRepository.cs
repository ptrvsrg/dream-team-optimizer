using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class PreferenceRepository: GenericRepository<Preference>, IPreferenceRepository
{
    public PreferenceRepository(AppDbContext dbContext): base(dbContext)
    {
    }
}