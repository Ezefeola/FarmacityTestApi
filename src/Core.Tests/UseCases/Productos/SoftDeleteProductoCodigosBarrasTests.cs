using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Models;
using Core.Contracts.Result;
using Core.Domain.Entities;
using Core.Tests.Abstractions;
using Core.UseCases.Productos;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;
public class SoftDeleteProductoCodigosBarrasTests : UseCaseTestBase<SoftDeleteProductoCodigosBarras>
{
    public SoftDeleteProductoCodigosBarrasTests() : base(unitOfWork => new SoftDeleteProductoCodigosBarras(unitOfWork)) { }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Success_When_CodigosBarras_Are_SoftDeleted()
    {
        int productoId = 1;
        int[] codigosBarrasIdsToDelete = [10, 11];

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

        ProductoRepositoryMock
            .Setup(r => r.GetProductoActivoByIdWithCodigosBarrasAsync(productoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(producto);

        UnitOfWorkMock
            .Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SaveResult { IsSuccess = true, RowsAffected = 2 });

        Result<SoftDeleteProductoCodigoDeBarraResponseDto> result =await UseCase.ExecuteAsync(productoId, requestDto, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.NotNull(result.Payload);

        Assert.All(producto.CodigosBarras
              .Where(x => codigosBarrasIdsToDelete.Contains(x.Id)), x => Assert.False(x.Activo));

        Assert.All(producto.CodigosBarras
              .Where(x => !codigosBarrasIdsToDelete.Contains(x.Id)), x => Assert.True(x.Activo));
    }
}