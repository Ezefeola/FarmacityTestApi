using Adapter.SqlServer.Data;
using Core.Contracts.Repositories;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adapter.SqlServer.Repositories;
public class CodigoBarraRepository : GenericRepository<CodigoBarra, int>, ICodigoBarraRepository
{
    public CodigoBarraRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> CodigoBarraActivoExistsAsync(string codigo, CancellationToken cancellationToken)
    {
        return await Query()
                     .AnyAsync(x => x.Codigo == codigo, cancellationToken);
    }

    public async Task<IEnumerable<CodigoBarra>> GetExistingCodigosBarrasAsync(
        int productoId,
        List<string> codigosBarras, 
        CancellationToken cancellationToken
    )
    {
        return await Query()
                     .IgnoreQueryFilters()
                     .Where(x => x.ProductoId == productoId)
                     .Where(x => codigosBarras.Contains(x.Codigo))
                     .ToListAsync(cancellationToken);
    }
}