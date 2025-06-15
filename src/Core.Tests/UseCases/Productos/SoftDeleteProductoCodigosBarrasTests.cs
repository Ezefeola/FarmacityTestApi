using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Models;
using Core.Contracts.Repositories;
using Core.Contracts.Result;
using Core.Contracts.UnitOfWork;
using Core.Domain.Entities;
using Core.UseCases.Productos;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;
public class SoftDeleteProductoCodigosBarrasTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductoRepository> _productoRepositoryMock;
    private readonly SoftDeleteProductoCodigosBarras _useCase;

    public SoftDeleteProductoCodigosBarrasTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productoRepositoryMock = new Mock<IProductoRepository>();

        _unitOfWorkMock.Setup(u => u.ProductoRepository).Returns(_productoRepositoryMock.Object);

        _useCase = new SoftDeleteProductoCodigosBarras(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Success_When_CodigosBarras_Are_SoftDeleted()
    {
        int productoId = 1;
        List<int> codigosBarrasIdsToDelete = [10, 11];

        SoftDeleteProductoCodigoDeBarraRequestDto requestDto = new()
        {
            CodigoBarraIds = codigosBarrasIdsToDelete
        };

        Producto producto = new(
            id: productoId,
            nombre: "Producto A",
            precio: 99,
            cantidadEnStock: 10,
            activo: true,
            fechaAlta: DateTime.UtcNow
        )
        {
            CodigosBarras =
            [
                new CodigoBarra(
                    id: 10, 
                    codigo: "AAA123", 
                    activo: true, 
                    fechaAlta: 
                    DateTime.UtcNow
                ),
                new CodigoBarra(
                    id: 11, 
                    codigo: "BBB456", 
                    activo: true, 
                    fechaAlta: DateTime.UtcNow
                ),
                new CodigoBarra(
                    id: 12, 
                    codigo: "CCC789", 
                    activo: true, 
                    fechaAlta: DateTime.UtcNow
                )
            ]
        };

        _productoRepositoryMock
            .Setup(r => r.GetProductoActivoByIdWithCodigoBarraAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        _unitOfWorkMock
            .Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SaveResult { IsSuccess = true, RowsAffected = 2 });

        Result<SoftDeleteProductoCodigoDeBarraResponseDto> result =await _useCase.ExecuteAsync(productoId, requestDto, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.NotNull(result.Payload);

        Assert.All(producto.CodigosBarras
              .Where(x => codigosBarrasIdsToDelete.Contains(x.Id)), x => Assert.False(x.Activo));

        Assert.All(producto.CodigosBarras
              .Where(x => !codigosBarrasIdsToDelete.Contains(x.Id)), x => Assert.True(x.Activo));
    }
}