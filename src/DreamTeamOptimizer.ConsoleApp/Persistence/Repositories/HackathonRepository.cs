using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.Core.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class HackathonRepository : GenericRepository<Hackathon>, IHackathonRepository
{
    private readonly AppDbContext _context;

    public HackathonRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public virtual double FindAverageResult()
    {
        return _context.Set<Hackathon>()
            .AsNoTracking()
            .Where(h => h.Status == HackathonStatus.COMPLETED)
            .Average(h => h.Result);
    }

    public override void Create(Hackathon item)
    {
        foreach (var itemEmployee in item.Employees)
        {
            _context.Attach(itemEmployee);
        }

        base.Create(item);
    }
}