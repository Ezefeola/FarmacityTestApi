using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Result;
using Core.Domain.Entities;
using Core.Tests.Abstractions;
using Core.UseCases.Productos;
using Core.Utilities.Validations;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;
public class GetProductosTests : UseCaseTestBase<GetProductos>
{
    public GetProductosTests() : base(unitOfWork => new GetProductos(unitOfWork)) { }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Success_When_Productos_Exist()
    {
        GetProductosRequestDto requestDto = new();

        List<Producto> productos =
        [
            new Producto(
                id: 1,
                nombre: "Producto1",
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
                        codigo: "ABC123",
                        activo: true,
                        fechaAlta: DateTime.UtcNow
                    )
                ]
            },
            new Producto(
                id: 2,
                nombre: "Producto2",
                precio: 200,
                cantidadEnStock: 3,
                activo: true,
                fechaAlta: DateTime.UtcNow
            )
            {
                CodigosBarras = []
            }
        ];

        ProductoRepositoryMock
            .Setup(r => r.GetAllProductosAsync(requestDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(productos);

        ProductoRepositoryMock
            .Setup(r => r.GetProductosCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(productos.Count);

        Result<GetProductosResponseDto> result = await UseCase.ExecuteAsync(requestDto, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.NotNull(result.Payload);
        Assert.Equal(productos.Count, result.Payload.Items.Count());

        List<ProductosResponseDto> items = result.Payload.Items.ToList();

        Assert.Equal(productos[0].Id, items[0].ProductoId);
        Assert.Equal(productos[0].Nombre, items[0].Nombre);
        Assert.Equal(productos[0].CodigosBarras.Count, items[0].CodigosBarras.Count);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_NotFound_When_Productos_Is_Null()
    {
        GetProductosRequestDto requestDto = new();

        ProductoRepositoryMock
                .Setup(x => x.GetAllProductosAsync(requestDto, It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult<IEnumerable<Producto>>(null!));

        Result<GetProductosResponseDto> result = await UseCase.ExecuteAsync(requestDto, default);

        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(ValidationMessages.Producto.PRODUCTOS_NOT_FOUND, result.Errors[0]);
    }
}