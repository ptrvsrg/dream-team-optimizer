using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    DbContext _context;
    DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public List<TEntity> Find()
    {
        return _dbSet.AsNoTracking().ToList();
    }

    public List<TEntity> Find(Func<TEntity, bool> predicate)
    {
        return _dbSet.AsNoTracking().Where(predicate).ToList();
    }

    public TEntity? FindById(int id)
    {
        return _dbSet.Find(id);
    }

    public void Create(TEntity item)
    {
        _dbSet.Add(item);
        _context.SaveChanges();
    }

    public void CreateAll(List<TEntity> items)
    {
        items.ForEach(item => _dbSet.Add(item));
        _context.SaveChanges();
    }

    public void Update(TEntity item)
    {
        var entry = _context.Entry(item);
        entry.State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void Remove(TEntity item)
    {
        _dbSet.Remove(item);
        _context.SaveChanges();
    }
}