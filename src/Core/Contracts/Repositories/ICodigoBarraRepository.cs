using Core.Domain.Entities;

namespace Core.Contracts.Repositories;
public interface ICodigoBarraRepository : IGenericRepository<CodigoBarra, int>
{
    Task<bool> CodigoBarraActivoExistsAsync(string codigo, CancellationToken cancellationToken);
    Task<IEnumerable<CodigoBarra>> GetExistingCodigosBarrasAsync(int productoId, List<string> codigosBarras, CancellationToken cancellationToken);
}