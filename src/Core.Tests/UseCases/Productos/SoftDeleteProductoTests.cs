using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Models;
using Core.Contracts.Repositories;
using Core.Contracts.Result;
using Core.Contracts.UnitOfWork;
using Core.Domain.Entities;
using Core.UseCases.Productos;
using Core.Utilities.Validations;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;
public class SoftDeleteProductoTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductoRepository> _productoRepositoryMock;
    private readonly SoftDeleteProducto _useCase;

    public SoftDeleteProductoTests()
    {
        _productoRepositoryMock = new Mock<IProductoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _unitOfWorkMock.Setup(u => u.ProductoRepository).Returns(_productoRepositoryMock.Object);

        _useCase = new SoftDeleteProducto(_unitOfWorkMock.Object);
    }

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

        _productoRepositoryMock
            .Setup(r => r.GetProductoByIdWithCodigosBarrasAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        _unitOfWorkMock
            .Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SaveResult { IsSuccess = true, RowsAffected = 1 });

        Result<SoftDeleteProductoResponseDto> result = await _useCase.ExecuteAsync(productoId, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.NotNull(result.Payload);
        Assert.False(result.Payload.Activo);
        Assert.Equal(productoId, result.Payload.Id);
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Producto_Does_Not_Exist()
    {
        // Arrange
        int productoId = 99;

        _productoRepositoryMock
            .Setup(r => r.GetProductoByIdWithCodigosBarrasAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Producto?)null);

        // Act
        Result<SoftDeleteProductoResponseDto> result = await _useCase.ExecuteAsync(productoId, default);

        // Assert
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

        _productoRepositoryMock
            .Setup(r => r.GetProductoByIdWithCodigosBarrasAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        _unitOfWorkMock
            .Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SaveResult { IsSuccess = false, ErrorMessage = "Error de persistencia" });

        Result<SoftDeleteProductoResponseDto> result = await _useCase.ExecuteAsync(productoId, default);

        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.InternalServerError, result.HttpStatusCode);
        Assert.Contains("Error de persistencia", result.Errors[0]);
    }
}