using Adapter.SqlServer.Data;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.Repositories;
using Core.Domain.Entities;
using Core.Utilities.QueryOptions;
using Microsoft.EntityFrameworkCore;

namespace Adapter.SqlServer.Repositories;
public class ProductoRepository : GenericRepository<Producto, int>, IProductoRepository
{
    public ProductoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Producto>> GetAllProductosAsync(
        GetProductosRequestDto parametersRequestDto, 
        CancellationToken cancellationToken
    )
    {
        return await Query()
                    .AsNoTracking()
                    .Include(x => x.CodigosBarras)
                    .ApplyPagination(
                        parametersRequestDto.GetPage(),
                        parametersRequestDto.GetPageSize()
                    )
                    .ToListAsync(cancellationToken);
    }

    public async Task<int> GetProductosCountAsync(CancellationToken cancellationToken)
    {
        return await Query()
                    .CountAsync();
    }

    public async Task<Producto?> GetProductoByIdWithCodigosBarrasAsync(int productoId, CancellationToken cancellationToken)
    {
        return await Query()
                     .IgnoreQueryFilters()
                     .Include(x => x.CodigosBarras)
                     .FirstOrDefaultAsync(x => x.Id == productoId, cancellationToken);
    }

    public async Task<Producto?> GetProductoByIdWithCodigosBarras(int productoId, CancellationToken cancellationToken)
    {
        return await Query()
                    .Where(x => x.Id == productoId)
                    .Include(x => x.CodigosBarras)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Producto?> GetProductoActivoByIdWithCodigoBarraAsync(int productoId, CancellationToken cancellationToken)
    {
        return await Query()
                     .Include(x => x.CodigosBarras)
                     .FirstOrDefaultAsync(x => x.Id == productoId, cancellationToken);
    }

    public async Task<bool> ProductoNombreActivoExistsAsync(string productoNombre, CancellationToken cancellationToken)
    {
        string normalizedNombre = productoNombre.Trim().ToLowerInvariant();
        return await Query()
                     .AsNoTracking()
                     .AnyAsync(x => x.Nombre.Trim().ToLower() == normalizedNombre, 
                        cancellationToken
                     );
    }
}