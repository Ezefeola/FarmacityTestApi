using Core.Contracts.Models;
using Core.Contracts.Repositories;
using Core.Contracts.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Adapter.SqlServer.Data;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public ICodigoBarraRepository CodigoBarraRepository { get; }
    public IProductoRepository ProductoRepository { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IProductoRepository productoRepository,
        ICodigoBarraRepository codigoBarraRepository
    )
    {
        _dbContext = context;
        ProductoRepository = productoRepository;
        CodigoBarraRepository = codigoBarraRepository;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
    }

    public async Task<SaveResult> CompleteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            int rowsAffected = await _dbContext.SaveChangesAsync(cancellationToken);

            return new SaveResult
            {
                IsSuccess = true,
                RowsAffected = rowsAffected
            };
        }
        catch (DbUpdateException ex)
        {
            return new SaveResult()
            {
                IsSuccess = false,
                ErrorMessage = ex.InnerException?.Message ?? ex.Message
            };
        }
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}