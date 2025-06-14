using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;

namespace Core.Contracts.UseCases.Productos
{
    public interface ICreateProducto
    {
        Task<Result<CreateProductoResponseDto>> ExecuteAsync(CreateProductoRequestDto requestDto, CancellationToken cancellationToken);
    }
}