using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;

namespace Core.Contracts.UseCases.Productos
{
    public interface ISoftDeleteProducto
    {
        Task<Result<SoftDeleteProductoResponseDto>> ExecuteAsync(int productoId, CancellationToken cancellationToken);
    }
}