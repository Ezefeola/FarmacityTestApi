using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Models;
using Core.Contracts.Result;
using Core.Domain.Entities;
using Core.Tests.Abstractions;
using Core.UseCases.Productos;
using Core.Utilities.Validations;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;
public class SoftDeleteProductoTests : UseCaseTestBase<SoftDeleteProducto>
{
    public SoftDeleteProductoTests() : base(unitOfWork => new SoftDeleteProducto(unitOfWork)) { }

    [Fact]
    public async Task Should_Return_Success_When_Producto_Exists()
    {
        int productoId = 1;

        Producto producto = new(
            id: productoId,
            nombre: "Producto A",
            precio: 150,
            cantidadEnStock: 10,
            activo: true,
            fechaAlta: DateTime.UtcNow
        )
        {
            CodigosBarras =
            [
                new CodigoBarra(
                    id: 1,
                    codigo: "XYZ123",
                    activo: true,
                    fechaAlta: DateTime.UtcNow
                )
            ]
        };

        ProductoRepositoryMock
            .Setup(r => r.GetProductoByIdWithCodigosBarrasAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        UnitOfWorkMock
            .Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SaveResult { IsSuccess = true, RowsAffected = 1 });

        Result<SoftDeleteProductoResponseDto> result = await UseCase.ExecuteAsync(productoId, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.NotNull(result.Payload);
        Assert.False(result.Payload.Activo);
        Assert.Equal(productoId, result.Payload.Id);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Producto_Does_Not_Exist()
    {
        int productoId = 99;

        ProductoRepositoryMock
            .Setup(r => r.GetProductoByIdWithCodigosBarrasAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Producto?)null);

        Result<SoftDeleteProductoResponseDto> result = await UseCase.ExecuteAsync(productoId, default);

        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.Contains(ValidationMessages.Producto.PRODUCTO_NOT_FOUND, result.Errors[0]);
    }

    [Fact]
    public async Task Should_Return_Failure_When_CompleteAsync_Fails()
    {
        int productoId = 2;

        Producto producto = new(
            id: productoId,
            nombre: "Producto B",
            precio: 99,
            cantidadEnStock: 4,
            activo: true,
            fechaAlta: DateTime.UtcNow
        );

        ProductoRepositoryMock
            .Setup(r => r.GetProductoByIdWithCodigosBarrasAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        UnitOfWorkMock
            .Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SaveResult { IsSuccess = false, ErrorMessage = "Error de persistencia" });

        Result<SoftDeleteProductoResponseDto> result = await UseCase.ExecuteAsync(productoId, default);

        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.InternalServerError, result.HttpStatusCode);
        Assert.Contains("Error de persistencia", result.Errors[0]);
    }
}