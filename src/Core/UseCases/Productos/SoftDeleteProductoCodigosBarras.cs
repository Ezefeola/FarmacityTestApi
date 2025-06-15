using Core.Contracts.DTOs.Productos.Request;
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
public class SoftDeleteProductoCodigosBarras : ISoftDeleteProductoCodigoBarra
{
    private readonly IUnitOfWork _unitOfWork;

    public SoftDeleteProductoCodigosBarras(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SoftDeleteProductoCodigoDeBarraResponseDto>> ExecuteAsync(
        int productoId,
        SoftDeleteProductoCodigoDeBarraRequestDto parametersRequestDto,
        CancellationToken cancellationToken
    )
    {
        if (parametersRequestDto.CodigoBarraIds.Count <= 0)
        {
            return Result<SoftDeleteProductoCodigoDeBarraResponseDto>.Failure(HttpStatusCode.BadRequest)
                                                                     .WithErrors([ValidationMessages.Producto.PRODUCTO_CODIGO_BARRA_NOT_VALID]);
        }

        Producto? producto = await _unitOfWork.ProductoRepository
                                                    .GetProductoActivoByIdWithCodigoBarraAsync
                                                    (
                                                        productoId, cancellationToken
                                                    );
        if (producto is null)
        {
            return Result<SoftDeleteProductoCodigoDeBarraResponseDto>.Failure(HttpStatusCode.NotFound)
                                                                     .WithErrors([
                                                                        $"{ValidationMessages.Producto.PRODUCTO_NOT_FOUND} " +
                                                                        $"(PRODUCTO_ID: {productoId})"
                                                                     ]);
        }

        List<CodigoBarra>? codigosBarrasToDelete = producto.CodigosBarras
                                                            .Where(
                                                                x => parametersRequestDto.CodigoBarraIds.Contains(x.Id)
                                                            )
                                                            .ToList();
        if (codigosBarrasToDelete.Count != parametersRequestDto.CodigoBarraIds.Count)
        {
            return Result<SoftDeleteProductoCodigoDeBarraResponseDto>.Failure(HttpStatusCode.NotFound)
                                                                     .WithErrors([
                                                                         $"{ValidationMessages.Producto.PRODUCTO_CODIGO_BARRA_NOT_VALID} " +
                                                                         $"(CODIGO_BARRA_IDS: {parametersRequestDto.CodigoBarraIds})"
                                                                     ]);
        }

        foreach (CodigoBarra codigoBarra in codigosBarrasToDelete)
        {
            codigoBarra.SoftDelete();
        }
        SaveResult saveResult = await _unitOfWork.CompleteAsync(cancellationToken);
        if (!saveResult.IsSuccess)
        {
            return Result<SoftDeleteProductoCodigoDeBarraResponseDto>.Failure(HttpStatusCode.InternalServerError)
                                                                     .WithErrors([saveResult.ErrorMessage]);
        }

        SoftDeleteProductoCodigoDeBarraResponseDto responseDto = producto.ToSoftDeleteProductoCodigoDeBarraResponseDto();
        return Result<SoftDeleteProductoCodigoDeBarraResponseDto>.Success(HttpStatusCode.OK)
                                                                 .WithDescription(
                                                                    $"The Codigos Barras where deleted successfully" +
                                                                    $"(CODIGO_BARRA_IDS {parametersRequestDto.CodigoBarraIds})"
                                                                 )
                                                                 .WithPayload(responseDto);
    }
}