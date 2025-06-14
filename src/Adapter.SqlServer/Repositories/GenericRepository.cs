using Adapter.SqlServer.Data;
using Core.Contracts.Models;
using Core.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Adapter.SqlServer.Repositories;
public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
where TEntity : class, IEntity<TId>
where TId : notnull
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(TId id, CancellationToken cancellationToken)
    {
        TEntity? entity = await GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.Activo = true;
        entity.FechaModificacion = DateTime.UtcNow;

        return true;
    }
}