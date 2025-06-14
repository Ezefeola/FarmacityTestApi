using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;

namespace Core.Contracts.UseCases.Productos;
public interface IGetProductoById
{
    Task<Result<GetProductoByIdResponseDto>> ExecuteAsync(int productoId, CancellationToken cancellationToken);
}