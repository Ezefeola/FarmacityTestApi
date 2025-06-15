using Adapter.SqlServer.Data;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.Repositories;
using Core.Domain.Entities;
using Core.Utilities.QueryOptions;
using Core.Utilities.QueryOptions.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Adapter.SqlServer.Repositories;
public class ProductoRepository : GenericRepository<Producto, int>, IProductoRepository
{
    public ProductoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Producto>> GetAllProductosAsync(
        GetProductosRequestDto parametersRequestDto, 
        CancellationToken cancellationToken
    )
    {
        IQueryable<Producto> getAllProductosQuery = Query()
                                                    .AsNoTracking()
                                                    .Include(x => x.CodigosBarras);

        Expression<Func<Producto, object>> sortFieldSelector = parametersRequestDto.SortColumn?.ToLower() switch
        {
            "nombre" => producto => producto.Nombre,
            "precio" => producto => producto.Precio,
            "fechaalta" => producto => producto.FechaAlta,
            _ => producto => producto.Id
        };
        if(parametersRequestDto.SortDescending == true)
        {
            getAllProductosQuery = getAllProductosQuery.OrderByDescending(sortFieldSelector);
        }
        else
        {
            getAllProductosQuery = getAllProductosQuery.OrderBy(sortFieldSelector);
        }

            return await getAllProductosQuery
                                    .ApplyFilteringToGetProductos(parametersRequestDto)
                                    .ApplyPagination(parametersRequestDto.GetPage(), parametersRequestDto.GetPageSize())
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

    public async Task<Producto?> GetProductoActivoByIdWithCodigosBarrasAsync(int productoId, CancellationToken cancellationToken)
    {
        return await Query()
                     .Where(x => x.Id == productoId)
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