using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;

namespace Core.Contracts.UseCases.Productos
{
    public interface ISoftDeleteProductoCodigoBarra
    {
        Task<Result<SoftDeleteProductoCodigoDeBarraResponseDto>> ExecuteAsync(
            int productoId,
            SoftDeleteProductoCodigoDeBarraRequestDto parametersRequestDto, 
            CancellationToken cancellationToken
        );
    }
}