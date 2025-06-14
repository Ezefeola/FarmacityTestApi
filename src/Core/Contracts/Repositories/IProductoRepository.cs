using Core.Contracts.DTOs.Productos.Request;
using Core.Domain.Entities;

namespace Core.Contracts.Repositories;
public interface IProductoRepository : IGenericRepository<Producto, int>
{
    Task<IEnumerable<Producto>> GetAllProductosAsync(GetProductosRequestDto parametersRequestDto, CancellationToken cancellationToken);
    Task<int> GetProductosCountAsync(CancellationToken cancellationToken);
}