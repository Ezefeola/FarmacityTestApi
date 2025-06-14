using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;

namespace Core.Contracts.UseCases.Productos
{
    public interface IUpdateProducto
    {
        Task<Result<UpdateProductoResponseDto>> ExecuteAsync(int productoId, UpdateProductoRequestDto requestDto, CancellationToken cancellationToken);
    }
}