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
}