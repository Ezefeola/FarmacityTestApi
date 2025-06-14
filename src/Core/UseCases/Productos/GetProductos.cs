using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;
using Core.Contracts.UnitOfWork;
using Core.Contracts.UseCases.Productos;
using Core.Domain.Entities;
using Core.Utilities.Mappers;
using Core.Utilities.Validations;
using System.Net;

namespace Core.UseCases.Productos;
public class GetProductos : IGetProductos
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductos(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetProductosResponseDto>> ExecuteAsync(
        GetProductosRequestDto parametersRequestDto,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<Producto> productos = await _unitOfWork.ProductoRepository.GetAllProductosAsync(parametersRequestDto, cancellationToken);
        if (productos is null)
        {
            return Result<GetProductosResponseDto>.Failure(HttpStatusCode.NotFound)
                                                  .WithErrors([ValidationMessages.Producto.PRODUCTOS_NOT_FOUND]);
        }

        int totalProductosCount = await _unitOfWork.ProductoRepository.GetProductosCountAsync(cancellationToken);
        GetProductosResponseDto responseDto = productos.ToGetProductosResponseDtos(
            parametersRequestDto,
            totalProductosCount
        );
        return Result<GetProductosResponseDto>.Success(HttpStatusCode.OK)
                                              .WithPayload(responseDto);
    }
}