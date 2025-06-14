using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;

namespace Core.Contracts.UseCases.Productos
{
    public interface IGetProductos
    {
        Task<Result<GetProductosResponseDto>> ExecuteAsync(GetProductosRequestDto parametersRequestDto, CancellationToken cancellationToken);
    }
}