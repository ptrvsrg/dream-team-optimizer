using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class HackathonRepository : GenericRepository<Hackathon>, IHackathonRepository
{
    public HackathonRepository(AppDbContext context) : base(context)
    {
    }
}