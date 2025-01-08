using CursusJapaneseLearningPlatform.Repository.Entities;
using System.Linq.Expressions;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task DeleteAsync(Guid id);
    Task UpdateAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetPageAsync(int pageNumber, int pageSize);
    IQueryable<TEntity> Entities { get; }
    IQueryable<TEntity> GetAllQueryable();
    Task<IQueryable<TEntity>> GetAllIQueryableAsync();
    Task<List<TEntity>> ToListAsync(IQueryable<TEntity> query, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAllAsync(object id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void Update(TEntity entity);
    //Task<PaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);

    Task<TEntity?> GetWithIncludesAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TEntity?> GetWithIncludesAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Func<IQueryable<TEntity>, IQueryable<TEntity>>[] includeProperties);
    
}
