using Core.Contracts.Models;

namespace Core.Contracts.Repositories;
public interface IGenericRepository<TEntity, TId>
where TEntity : class, IEntity<TId>
where TId : notnull
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken);
    void Add(TEntity entity);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    IQueryable<TEntity> Query();
    Task<bool> SoftDeleteAsync(TId id, CancellationToken cancellationToken);
}