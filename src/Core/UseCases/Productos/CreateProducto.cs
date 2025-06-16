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
public class CreateProducto : ICreateProducto
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateProductoRequestDto> _validator;

    public CreateProducto(
        IUnitOfWork unitOfWork,
        IValidator<CreateProductoRequestDto> validator
    )
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<CreateProductoResponseDto>> ExecuteAsync(
        CreateProductoRequestDto requestDto,
        CancellationToken cancellationToken
    )
    {
        ValidationResult validatorResult = _validator.Validate(requestDto);
        if(!validatorResult.IsValid)
        {
            return Result<CreateProductoResponseDto>.Failure(HttpStatusCode.BadRequest)
                                                    .WithErrors(validatorResult.Errors.Select(x => x.ErrorMessage).ToList());    
        }
        bool productoNombreExists = await _unitOfWork.ProductoRepository.ProductoNombreActivoExistsAsync(requestDto.Nombre, cancellationToken);
        if(productoNombreExists)
        {
            return Result<CreateProductoResponseDto>.Failure(HttpStatusCode.BadRequest)
                                                    .WithErrors([
                                                        $"{ValidationMessages.Producto.PRODUCTO_NOMBRE_ACTIVO_EXISTS} " +
                                                        $"(NOMBRE: {requestDto.Nombre})"
                                                    ]);
        }

        List<string> codigosBarras = [];
        if(requestDto.CodigosBarras.Count > 0)
        {
            codigosBarras = requestDto.CodigosBarras
                                                .Select(x => x.Codigo.Trim())
                                                .ToList();
            foreach (string codigoBarra in codigosBarras)
            {
                bool exists = await _unitOfWork.CodigoBarraRepository.CodigoBarraActivoExistsAsync(codigoBarra, cancellationToken);
                if (exists)
                {
                    return Result<CreateProductoResponseDto>.Failure(HttpStatusCode.BadRequest)
                                                            .WithErrors([
                                                                $"{ValidationMessages.CodigoBarra.CODIGO_BARRA_EXISTS} " +
                                                                $"(CODIGO_BARRA: {codigoBarra})"
                                                            ]);
                }
            }

        }

        Producto producto = requestDto.ToEntity(codigosBarras);
        await _unitOfWork.ProductoRepository.AddAsync(producto, cancellationToken);
        SaveResult saveResult = await _unitOfWork.CompleteAsync(cancellationToken);
        if (!saveResult.IsSuccess)
        {
            return Result<CreateProductoResponseDto>.Failure(HttpStatusCode.InternalServerError)
                                                    .WithErrors([saveResult.ErrorMessage]);
        }

        CreateProductoResponseDto responseDto = producto.ToCreateProductoResponseDto();
        return Result<CreateProductoResponseDto>.Success(HttpStatusCode.Created)
                                                .WithPayload(responseDto);
    }
}