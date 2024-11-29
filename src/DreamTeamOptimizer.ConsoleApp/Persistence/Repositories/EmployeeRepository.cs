using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.Core.Persistence.Repositories;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context)
    {
    }
}