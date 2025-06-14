using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
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

        List<string> codigosBarras = requestDto.CodigosBarras
                                            .Select(cb => cb.Codigo.Trim())
                                            .ToList();
        foreach (string? codigo in codigosBarras)
        {
            bool exists = await _unitOfWork.CodigoBarraRepository.CodigoBarraExistsAsync(codigo, cancellationToken);
            if (exists)
            {
                return Result<CreateProductoResponseDto>.Success(HttpStatusCode.BadRequest)
                                                        .WithErrors([$"{ValidationMessages.CodigoBarra.CODIGO_BARRA_EXISTS} : {codigo}"]);
            }
        }

        Producto producto = requestDto.ToEntity(codigosBarras);
        await _unitOfWork.ProductoRepository.AddAsync(producto, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        CreateProductoResponseDto responseDto = producto.ToCreateProductoResponseDto();
        return Result<CreateProductoResponseDto>.Success(HttpStatusCode.OK)
                                                .WithPayload(responseDto);
    }
}