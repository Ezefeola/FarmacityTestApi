
using Core.Domain.Entities;

namespace Core.Contracts.Repositories;
public interface ICodigoBarraRepository : IGenericRepository<CodigoBarra, int>
{
    Task<bool> CodigoBarraExistsAsync(string codigo, CancellationToken cancellationToken);
}