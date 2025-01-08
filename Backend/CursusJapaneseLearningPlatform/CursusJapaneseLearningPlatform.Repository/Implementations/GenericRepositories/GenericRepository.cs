using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CursusJapaneseLearningPlatform.Repository.Bases.BaseEntitys;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
namespace CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext dbContext)
    {
        _context = dbContext;
    }

    public IQueryable<T> Entities => _context.Set<T>().Where(e => e.IsActive && !e.IsDelete);

    public IQueryable<T> GetAllQueryable()
    {
        return Entities;
    }

    public async Task<List<T>> ToListAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        return await query.ToListAsync(cancellationToken);
    }

    public Task<IQueryable<T>> GetAllIQueryableAsync()
    {
        return Task.FromResult(Entities);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Entities.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null && entity.IsActive && !entity.IsDelete)
        {
            return entity;
        }
        return null;
    }
    public async Task<T?> GetByIdAllAsync(object id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        if (entities == null || !entities.Any())
        {
            throw new ArgumentException("The entities list cannot be null or empty.", nameof(entities));
        }

        foreach (var entity in entities)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
            }
            await _context.Set<T>().AddAsync(entity);
        }

        await _context.SaveChangesAsync();
        return await Entities.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<T>().FindAsync(id, cancellationToken);
        if (entity != null && entity.IsActive && !entity.IsDelete)
        {
            return entity;
        }
        return null;
    }

    public async Task<T> AddAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _context.Set<T>().Attach(entity);
        }
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task<T?> GetWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _context.Set<T>();

        query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> GetWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Func<IQueryable<T>, IQueryable<T>>[] includeProperties)
    {
        IQueryable<T> query = _context.Set<T>();

        query = includeProperties.Aggregate(query, (current, includeProperty) => includeProperty(current));

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetPageAsync(int pageNumber, int pageSize)
    {
        return await _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }
    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null && entity.IsActive && !entity.IsDelete)
        {
            return entity;
        }
        return null;
    }
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public virtual async Task UpdateAsync(T entity)
    {
        var existingEntity = await _context.Set<T>().FindAsync(entity.Id);
        if (existingEntity == null)
        {
            throw new ArgumentException($"{typeof(T).Name} not found");
        }

       
        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        existingEntity.LastUpdatedTime = DateTime.UtcNow;
        existingEntity.LastUpdatedBy = "User"; 

        
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            
        }
    }
    
}
