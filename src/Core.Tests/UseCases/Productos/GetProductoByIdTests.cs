using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Repositories;
using Core.Contracts.Result;
using Core.Contracts.UnitOfWork;
using Core.Domain.Entities;
using Core.UseCases.Productos;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;
public class GetProductoByIdTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductoRepository> _productoRepositoryMock;
    private readonly GetProductoById _useCase;

    public GetProductoByIdTests()
    {
        _productoRepositoryMock = new Mock<IProductoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock.Setup(x => x.ProductoRepository)
                       .Returns(_productoRepositoryMock.Object);

        _useCase = new GetProductoById(_unitOfWorkMock.Object);
    }

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

        _productoRepositoryMock
            .Setup(x => x.GetProductoByIdWithCodigosBarras(productoId, default))
            .ReturnsAsync(producto);

        Result<GetProductoByIdResponseDto> result = await _useCase.ExecuteAsync(productoId, default);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);
        Assert.Equal("Producto Test", result.Payload.Nombre);
        Assert.Equal(100, result.Payload.Precio);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Producto_Does_Not_Exist()
    {
        int productoId = 42;

        _productoRepositoryMock
            .Setup(x => x.GetProductoByIdWithCodigosBarras(productoId, default))
            .ReturnsAsync((Producto?)null);

        Result<GetProductoByIdResponseDto> result = await _useCase.ExecuteAsync(productoId, default);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Payload);
        Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Contains($"(PRODUCTO_ID: {productoId})", result.Errors[0]);
    }
}