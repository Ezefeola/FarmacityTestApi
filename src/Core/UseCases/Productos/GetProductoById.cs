using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;
using Core.Contracts.UnitOfWork;
using Core.Contracts.UseCases.Productos;
using Core.Domain.Entities;
using Core.Utilities.Mappers;
using Core.Utilities.Validations;
using System.Net;

namespace Core.UseCases.Productos;
public class GetProductoById : IGetProductoById
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductoById(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetProductoByIdResponseDto>> ExecuteAsync(
        int productoId,
        CancellationToken cancellationToken
    )
    {
        Producto? producto = await _unitOfWork.ProductoRepository.GetProductoActivoByIdWithCodigosBarrasAsync(productoId, cancellationToken);
        if (producto is null)
        {
            return Result<GetProductoByIdResponseDto>.Failure(HttpStatusCode.NotFound)
                                                     .WithErrors([
                                                        $"{ValidationMessages.Producto.PRODUCTO_NOT_FOUND} " +
                                                        $"(PRODUCTO_ID: {productoId})"
                                                     ]);
        }

        GetProductoByIdResponseDto responseDto = producto.ToGetProductoByIdResponseDto();
        return Result<GetProductoByIdResponseDto>.Success(HttpStatusCode.OK)
                                                 .WithPayload(responseDto);
    }
}