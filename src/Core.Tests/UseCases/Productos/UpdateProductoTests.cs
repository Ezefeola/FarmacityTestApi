using Core.Contracts.DTOs.CodigosBarras.Request;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Models;
using Core.Contracts.Result;
using Core.Domain.Entities;
using Core.Tests.Abstractions;
using Core.UseCases.Productos;
using FluentValidation.Results;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;
public class UpdateProductoTests : UseCaseTestBase<UpdateProducto, UpdateProductoRequestDto>
{

    public UpdateProductoTests() : base((unitOfWork, validator) => new UpdateProducto(unitOfWork, validator))
    {
    }

    [Fact]
    public async Task Should_Return_Success_When_Update_Is_Valid()
    {
        int productoId = 1;

        UpdateProductoRequestDto requestDto = new()
        {
            Nombre = "Producto Actualizado",
            Precio = 200,
            CantidadEnStock = 10,
            CodigosBarras =
            [
                new UpdateCodigoBarraRequestDto
            {
                Codigo = "ABC123"
            }
            ]
        };

        Producto producto = new(
            id: productoId,
            nombre: "Producto Original",
            precio: 100,
            cantidadEnStock: 5,
            activo: true,
            fechaAlta: DateTime.UtcNow
        )
        {
            CodigosBarras =
            [
                new CodigoBarra(
                id: 1,
                codigo: "XYZ999",
                activo: true,
                fechaAlta: DateTime.UtcNow
            )
            ]
        };

        List<CodigoBarra> existingCodigosBarras = new()
    {
        new CodigoBarra(id: 2, codigo: "ABC123", activo: false, fechaAlta: DateTime.UtcNow)
    };

        ValidatorMock
            .Setup(v => v.Validate(requestDto))
            .Returns(new ValidationResult());

        ProductoRepositoryMock
            .Setup(r => r.GetProductoByIdWithCodigosBarrasAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        ProductoRepositoryMock
            .Setup(r => r.ProductoNombreActivoExistsAsync(requestDto.Nombre, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        CodigoBarraRepositoryMock
            .Setup(r => r.GetExistingCodigosBarrasAsync(productoId, It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCodigosBarras);

        UnitOfWorkMock
            .Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SaveResult { IsSuccess = true });

        Result<UpdateProductoResponseDto> result = await UseCase.ExecuteAsync(productoId, requestDto, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.NotNull(result.Payload);
        Assert.Equal(requestDto.Nombre, result.Payload.Nombre);
        Assert.Equal(requestDto.Precio, result.Payload.Precio);
        Assert.Equal(requestDto.CantidadEnStock, result.Payload.CantidadEnStock);
        Assert.Equal(2, result.Payload.CodigosBarras.Count);
    }
}
