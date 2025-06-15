using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Models;
using Core.Contracts.Result;
using Core.Contracts.UnitOfWork;
using Core.Contracts.UseCases.Productos;
using Core.Domain.Entities;
using Core.Utilities.Mappers;
using Core.Utilities.Validations;
using System.Net;

namespace Core.UseCases.Productos;
public class SoftDeleteProducto : ISoftDeleteProducto
{
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteProducto(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SoftDeleteProductoResponseDto>> ExecuteAsync(
        int productoId,
        CancellationToken cancellationToken
    )
    {
        Producto? producto = await _unitOfWork.ProductoRepository.GetProductoByIdWithCodigosBarrasAsync(productoId, cancellationToken);
        if (producto is null)
        {
            return Result<SoftDeleteProductoResponseDto>.Failure(HttpStatusCode.NotFound)
                                                        .WithErrors([$"{ValidationMessages.Producto.PRODUCTO_NOT_FOUND} (ID: {productoId})"]);
        }

        producto.SoftDelete();
        SaveResult saveResult = await _unitOfWork.CompleteAsync(cancellationToken);
        if (!saveResult.IsSuccess)
        {
            return Result<SoftDeleteProductoResponseDto>.Failure(HttpStatusCode.InternalServerError)
                                                        .WithErrors([saveResult.ErrorMessage]);
        }

        SoftDeleteProductoResponseDto responseDto = producto.ToSoftDeleteProductoResponseDto();
        return Result<SoftDeleteProductoResponseDto>.Success(HttpStatusCode.OK)
                                                    .WithPayload(responseDto);
    }
}