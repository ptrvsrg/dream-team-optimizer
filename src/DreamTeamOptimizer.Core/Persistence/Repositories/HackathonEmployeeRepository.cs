using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence.Entities;

namespace DreamTeamOptimizer.Core.Persistence.Repositories;

public class HackathonEmployeeRepository : GenericRepository<HackathonEmployee>, IHackathonEmployeeRepository
{
    public HackathonEmployeeRepository(AppDbContext context) : base(context)
    {
    }
}