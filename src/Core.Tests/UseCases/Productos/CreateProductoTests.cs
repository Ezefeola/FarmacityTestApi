using Core.Contracts.DTOs.CodigosBarras.Request;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Models;
using Core.Contracts.Result;
using Core.Domain.Entities;
using Core.Tests.Abstractions;
using Core.UseCases.Productos;
using Core.Utilities.Validations;
using FluentValidation.Results;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;

public class CreateProductoTests : UseCaseTestBase<CreateProducto, CreateProductoRequestDto>
{

    public CreateProductoTests() : base((unitOfWork, validator) => new CreateProducto(unitOfWork, validator))
    {
    }

    [Fact]
    public async Task Should_Return_BadRequest_When_Validation_Fails()
    {
        CreateProductoRequestDto request = new()
        {
            Nombre = "",
            Precio = -1
        };
        List<ValidationFailure> failures = new()
        {
            new ValidationFailure("Nombre", "Nombre no puede estar vacío")
        };

        ValidatorMock.Setup(v => v.Validate(request)).Returns(new ValidationResult(failures));

        Result<CreateProductoResponseDto> result = await UseCase.ExecuteAsync(request, default);

        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Nombre no puede estar vacío", result.Errors);
    }

    [Fact]
    public async Task Should_Return_BadRequest_When_ProductoNombre_Exists()
    {
        CreateProductoRequestDto requestDto = new()
        {
            Nombre = "Producto Existente",
            Precio = 50,
            CantidadEnStock = 5,
            CodigosBarras = []
        };
        ValidatorMock.Setup(v => v.Validate(It.IsAny<CreateProductoRequestDto>()))
              .Returns(new ValidationResult());

        UnitOfWorkMock.Setup(u => u.ProductoRepository.ProductoNombreActivoExistsAsync(
            requestDto.Nombre,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        Result<CreateProductoResponseDto> result = await UseCase.ExecuteAsync(requestDto, default);

        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.Contains(ValidationMessages.Producto.PRODUCTO_NOMBRE_ACTIVO_EXISTS, result.Errors[0]);
    }
    [Fact]
    public async Task Should_Return_BadRequest_When_CodigoBarra_Exists()
    {
        CreateProductoRequestDto requestDto = new()
        {
            Nombre = "Producto Nuevo",
            Precio = 50,
            CantidadEnStock = 5,
            CodigosBarras =
            [
                new() { Codigo = "12345" },
                new() { Codigo = "67890" }
            ]
        };

        ValidatorMock.Setup(v => v.Validate(It.IsAny<CreateProductoRequestDto>()))
                      .Returns(new ValidationResult());

        ProductoRepositoryMock.Setup(r => r.ProductoNombreActivoExistsAsync(
            requestDto.Nombre, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        CodigoBarraRepositoryMock.Setup(r => r.CodigoBarraActivoExistsAsync(
            "12345", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        CodigoBarraRepositoryMock.Setup(r => r.CodigoBarraActivoExistsAsync(
            "67890", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Result<CreateProductoResponseDto> result = await UseCase.ExecuteAsync(requestDto, default);

        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.Contains(ValidationMessages.CodigoBarra.CODIGO_BARRA_EXISTS, result.Errors[0]);
    }

    [Fact]
    public async Task Should_Create_Producto_When_Data_Is_Valid()
    {
        CreateProductoRequestDto request = new()
        {
            Nombre = "ProductoNuevo",
            Precio = 50,
            CantidadEnStock = 5,
            CodigosBarras = [
                new CreateCodigoBarraRequestDto
                {
                    Codigo = "111222333"
                }
            ]
        };

        ValidatorMock.Setup(v => v.Validate(request)).Returns(new ValidationResult());

        ProductoRepositoryMock.Setup(r => r.ProductoNombreActivoExistsAsync(request.Nombre, default))
                              .ReturnsAsync(false);

        CodigoBarraRepositoryMock.Setup(r => r.CodigoBarraActivoExistsAsync(It.IsAny<string>(), default))
                                 .ReturnsAsync(false);

        ProductoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Producto>(), default))
                              .Returns(Task.CompletedTask);

        UnitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new SaveResult
                       {
                           IsSuccess = true,
                           RowsAffected = 1,
                           ErrorMessage = null
                       });

        Result<CreateProductoResponseDto> result = await UseCase.ExecuteAsync(request, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.Created, result.HttpStatusCode);
        Assert.NotNull(result.Payload);
        Assert.Equal(request.Nombre, result.Payload.Nombre);

        ProductoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Producto>(), default), Times.Once);
        UnitOfWorkMock.Verify(u => u.CompleteAsync(default), Times.Once);
    }
}