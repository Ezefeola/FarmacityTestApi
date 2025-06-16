using Core.Contracts.DTOs.CodigosBarras.Request;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Models;
using Core.Contracts.Result;
using Core.Contracts.UnitOfWork;
using Core.Contracts.UseCases.Productos;
using Core.Domain.Entities;
using Core.Utilities.Mappers;
using Core.Utilities.Validations;
using FluentValidation;
using FluentValidation.Results;
using System.Net;

namespace Core.UseCases.Productos;
public class UpdateProducto : IUpdateProducto
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateProductoRequestDto> _validator;

    public UpdateProducto(
        IUnitOfWork unitOfWork,
        IValidator<UpdateProductoRequestDto> validator
    )
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<UpdateProductoResponseDto>> ExecuteAsync(
        int productoId,
        UpdateProductoRequestDto requestDto,
        CancellationToken cancellationToken
    )
    {
        ValidationResult validatorResult = _validator.Validate(requestDto);
        if (!validatorResult.IsValid)
        {
            return Result<UpdateProductoResponseDto>.Failure(HttpStatusCode.BadRequest)
                                                    .WithErrors(validatorResult.Errors.Select(x => x.ErrorMessage).ToList());
        }
        if (!string.IsNullOrWhiteSpace(requestDto.Nombre))
        {
            bool nombreExists = await _unitOfWork.ProductoRepository.ProductoNombreActivoExistsAsync(requestDto.Nombre, cancellationToken);
            if (nombreExists)
            {
                return Result<UpdateProductoResponseDto>.Failure(HttpStatusCode.BadRequest)
                                                        .WithErrors([
                                                            $"{ValidationMessages.Producto.PRODUCTO_NOMBRE_ACTIVO_EXISTS} " +
                                                            $"(NOMBRE: {requestDto.Nombre})"
                                                        ]);
            }
        }

        Producto? producto = await _unitOfWork.ProductoRepository.GetProductoByIdWithCodigosBarrasAsync(productoId, cancellationToken);
        if (producto is null)
        {
            return Result<UpdateProductoResponseDto>.Failure(HttpStatusCode.NotFound)
                                                    .WithErrors([ 
                                                        $"{ValidationMessages.Producto.PRODUCTO_NOT_FOUND}" +
                                                        $" (PRODUCTO_ID: {productoId})"
                                                    ]);
        }

        List<UpdateCodigoBarraRequestDto> codigosBarrasToUpdate = requestDto.CodigosBarras
                                                                                    .Where(x => x.CodigoBarraId.HasValue)
                                                                                    .ToList();
        foreach (UpdateCodigoBarraRequestDto codigoBarra in codigosBarrasToUpdate)
        {
            CodigoBarra? existingCodigoBarra = producto.CodigosBarras.FirstOrDefault(x => x.Id == codigoBarra.CodigoBarraId);
            if (existingCodigoBarra is null)
            {
                return Result<UpdateProductoResponseDto>.Failure(HttpStatusCode.BadRequest)
                                                        .WithErrors([
                                                            $"{ValidationMessages.CodigoBarra.CODIGO_BARRA_NOT_FOUND}" +
                                                                $" (CODIGO_BARRA_ID:{codigoBarra.CodigoBarraId})"
                                                        ]);
            }

            bool isSameCodigoBarra = existingCodigoBarra.Codigo.Equals(codigoBarra.Codigo, StringComparison.OrdinalIgnoreCase);
            if (!isSameCodigoBarra)
            {
                bool codigoBarraActivoExists = await _unitOfWork.CodigoBarraRepository.CodigoBarraActivoExistsAsync(codigoBarra.Codigo, cancellationToken);
                if (codigoBarraActivoExists)
                {
                    return Result<UpdateProductoResponseDto>.Failure(HttpStatusCode.BadRequest)
                                                            .WithErrors([
                                                                $"{ValidationMessages.CodigoBarra.CODIGO_BARRA_EXISTS} " +
                                                                $"(CODIGO: {codigoBarra.Codigo})"
                                                            ]);
                }
            }
            existingCodigoBarra.UpdateIfChanged(codigoBarra.Codigo);
        }

        List<string> newCodigosBarras = requestDto.CodigosBarras
                                                            .Where(x => !x.CodigoBarraId.HasValue)
                                                            .Select(x => x.Codigo.Trim())
                                                            .ToList();
        IEnumerable<CodigoBarra> existingCodigosBarras = await _unitOfWork.CodigoBarraRepository
                                                                                    .GetExistingCodigosBarrasAsync(
                                                                                        productoId,
                                                                                        newCodigosBarras,
                                                                                        cancellationToken
                                                                                    );
        foreach (string codigoBarra in newCodigosBarras)
        {
            CodigoBarra? existingCodigoBarra = existingCodigosBarras.FirstOrDefault(c => c.Codigo.Equals(codigoBarra, StringComparison.OrdinalIgnoreCase));
            if (existingCodigoBarra is not null)
            {
                if (existingCodigoBarra.Activo)
                {
                    return Result<UpdateProductoResponseDto>.Failure(HttpStatusCode.BadRequest)
                                                            .WithErrors([
                                                                $"{ValidationMessages.CodigoBarra.CODIGO_BARRA_EXISTS}" +
                                                                        $" (CODIGO_BARRA: {codigoBarra})"
                                                            ]);
                }

                existingCodigoBarra.Activo = true;
                existingCodigoBarra.FechaModificacion = DateTime.UtcNow;
            }
        }

        if(newCodigosBarras.Count > 0)
        {
            producto.AddNewCodigosBarras(newCodigosBarras);
        }
        producto.UpdateIfChanged(
            requestDto.Nombre, 
            requestDto.Precio,
            requestDto.CantidadEnStock
        );
        SaveResult saveResult = await _unitOfWork.CompleteAsync(cancellationToken);
        if (!saveResult.IsSuccess)
        {
            return Result<UpdateProductoResponseDto>.Failure(HttpStatusCode.InternalServerError)
                                                    .WithErrors([saveResult.ErrorMessage]);
        }

        UpdateProductoResponseDto responseDto = producto.ToUpdateProductoResponseDto();
        return Result<UpdateProductoResponseDto>.Success(HttpStatusCode.OK)
                                                .WithPayload(responseDto);
    }
}