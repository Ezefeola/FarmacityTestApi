using Core.Contracts.Models;
using Core.Contracts.Repositories;

namespace Core.Contracts.UnitOfWork;
public interface IUnitOfWork
{
    public ICodigoBarraRepository CodigoBarraRepository { get; }
    public IProductoRepository ProductoRepository { get; }

    public Task<SaveResult> CompleteAsync(CancellationToken cancellationToken = default);
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}