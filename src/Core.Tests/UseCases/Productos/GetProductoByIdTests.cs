using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Repositories;
using Core.Contracts.Result;
using Core.Contracts.UnitOfWork;
using Core.Domain.Entities;
using Core.Tests.Abstractions;
using Core.UseCases.Productos;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;
public class GetProductoByIdTests : UseCaseTestBase<GetProductoById>
{
    public GetProductoByIdTests() : base(unitOfWork => new GetProductoById(unitOfWork)) { }


    [Fact]
    public async Task Should_Return_Producto_When_Exists()
    {
        int productoId = 1;
        Producto producto = new(
            id: productoId,
            nombre : "Producto Test",
            precio : 100,
            cantidadEnStock : 10,
            activo : true,
            fechaAlta : DateTime.UtcNow
        );

        ProductoRepositoryMock
            .Setup(x => x.GetProductoActivoByIdWithCodigosBarrasAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        Result<GetProductoByIdResponseDto> result = await UseCase.ExecuteAsync(productoId, default);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);
        Assert.Equal("Producto Test", result.Payload.Nombre);
        Assert.Equal(100, result.Payload.Precio);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Producto_Does_Not_Exist()
    {
        int productoId = 42;

        ProductoRepositoryMock
            .Setup(x => x.GetProductoActivoByIdWithCodigosBarrasAsync(productoId, default))
            .ReturnsAsync((Producto?)null);

        Result<GetProductoByIdResponseDto> result = await UseCase.ExecuteAsync(productoId, default);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Payload);
        Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Contains($"(PRODUCTO_ID: {productoId})", result.Errors[0]);
    }
}