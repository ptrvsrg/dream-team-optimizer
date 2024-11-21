namespace DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    void Create(TEntity item);
    void CreateAll(List<TEntity> items);
    void Update(TEntity item);
    void Remove(TEntity item);
    TEntity? FindById(int id);
    List<TEntity> Find();
    List<TEntity> Find(Func<TEntity, bool> predicate);
}