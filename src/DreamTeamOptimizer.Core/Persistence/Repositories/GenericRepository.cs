using DreamTeamOptimizer.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.Core.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual List<TEntity> FindAll()
    {
        return _dbSet.ToList();
    }

    public virtual List<TEntity> Find(Func<TEntity, bool> predicate)
    {
        return _dbSet.Where(predicate).ToList();
    }

    public virtual TEntity? FindById(object id)
    {
        return _dbSet.Find(id);
    }

    public virtual void Create(TEntity item)
    {
        _dbSet.Add(item);
        _context.SaveChanges();
    }

    public virtual void CreateAll(List<TEntity> items)
    {
        _dbSet.AddRange(items);
        _context.SaveChanges();
    }

    public virtual void Update(TEntity item)
    {
        _context.Attach(item);
        _context.Entry(item).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public virtual void Remove(TEntity item)
    {
        if (_context.Entry(item).State == EntityState.Detached) _dbSet.Attach(item);

        _dbSet.Remove(item);
        _context.SaveChanges();
    }

    public virtual void Remove(object id)
    {
        var item = _dbSet.Find(id)!;
        Remove(item);
    }
}