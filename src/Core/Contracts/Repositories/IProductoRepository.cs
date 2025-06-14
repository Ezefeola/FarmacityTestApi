using Core.Contracts.DTOs.Productos.Request;
using Core.Domain.Entities;

namespace Core.Contracts.Repositories;
public interface IProductoRepository : IGenericRepository<Producto, int>
{
    Task<IEnumerable<Producto>> GetAllProductosAsync(GetProductosRequestDto parametersRequestDto, CancellationToken cancellationToken);
    Task<Producto?> GetProductoByIdWithCodigosBarrasAsync(int productoId, CancellationToken cancellationToken);
    Task<Producto?> GetProductoActivoByIdWithCodigoBarraAsync(int productoId, CancellationToken cancellationToken);
    Task<int> GetProductosCountAsync(CancellationToken cancellationToken);
    Task<bool> ProductoNombreActivoExistsAsync(string nombre, CancellationToken cancellationToken);
    Task<Producto?> GetProductoByIdWithCodigosBarras(int productoId, CancellationToken cancellationToken);
}