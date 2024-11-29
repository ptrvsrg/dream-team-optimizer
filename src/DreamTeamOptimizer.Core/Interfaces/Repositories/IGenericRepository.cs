namespace DreamTeamOptimizer.Core.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    void Create(TEntity item);
    void CreateAll(List<TEntity> items);
    void Update(TEntity item);
    void Remove(TEntity item);
    void Remove(object id);
    TEntity? FindById(object id);
    List<TEntity> FindAll();
    List<TEntity> Find(Func<TEntity, bool> predicate);
}