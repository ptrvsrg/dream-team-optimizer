using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.Core.Persistence.Repositories;

public class HackathonRepository : GenericRepository<Hackathon>, IHackathonRepository
{
    private readonly AppDbContext _context;

    public HackathonRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public virtual double FindAverageResult()
    {
        var hackathons = _context.Set<Hackathon>()
            .AsNoTracking()
            .Where(h => h.Status == HackathonStatus.COMPLETED);
        if (!hackathons.Any()) return 0;
        return hackathons.Average(h => h.Result);
    }

    public override void Create(Hackathon item)
    {
        foreach (var itemEmployee in item.Employees) _context.Attach(itemEmployee);

        base.Create(item);
    }
}