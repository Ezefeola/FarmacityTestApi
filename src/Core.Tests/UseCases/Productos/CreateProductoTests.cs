using Core.Contracts.DTOs.CodigosBarras.Request;
using Core.Contracts.DTOs.Productos.Request;
using Core.Contracts.DTOs.Productos.Response;
using Core.Contracts.Models;
using Core.Contracts.Repositories;
using Core.Contracts.Result;
using Core.Contracts.UnitOfWork;
using Core.Domain.Entities;
using Core.UseCases.Productos;
using Core.Utilities.Validations;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Net;

namespace Core.Tests.UseCases.Productos;
public class CreateProductoTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductoRepository> _productoRepositoryMock;
    private readonly Mock<ICodigoBarraRepository> _codigoBarraRepositoryMock;
    private readonly Mock<IValidator<CreateProductoRequestDto>> _validatorMock;
    private readonly CreateProducto _useCase;

    public CreateProductoTests()
    {
        _productoRepositoryMock = new Mock<IProductoRepository>();
        _codigoBarraRepositoryMock = new Mock<ICodigoBarraRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validatorMock = new Mock<IValidator<CreateProductoRequestDto>>();

        _unitOfWorkMock.Setup(u => u.ProductoRepository).Returns(_productoRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.CodigoBarraRepository).Returns(_codigoBarraRepositoryMock.Object);

        _useCase = new CreateProducto(_unitOfWorkMock.Object, _validatorMock.Object);
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

        _validatorMock.Setup(v => v.Validate(request)).Returns(new ValidationResult(failures));

        var result = await _useCase.ExecuteAsync(request, default);

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
        _validatorMock.Setup(v => v.Validate(It.IsAny<CreateProductoRequestDto>()))
              .Returns(new ValidationResult());

        _unitOfWorkMock.Setup(u => u.ProductoRepository.ProductoNombreActivoExistsAsync(
            requestDto.Nombre,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var result = await _useCase.ExecuteAsync(requestDto, default);

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

        _validatorMock.Setup(v => v.Validate(It.IsAny<CreateProductoRequestDto>()))
                      .Returns(new ValidationResult());

        _productoRepositoryMock.Setup(r => r.ProductoNombreActivoExistsAsync(
            requestDto.Nombre, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _codigoBarraRepositoryMock.Setup(r => r.CodigoBarraActivoExistsAsync(
            "12345", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _codigoBarraRepositoryMock.Setup(r => r.CodigoBarraActivoExistsAsync(
            "67890", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Result<CreateProductoResponseDto> result = await _useCase.ExecuteAsync(requestDto, default);

        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
        Assert.Contains(ValidationMessages.CodigoBarra.CODIGO_BARRA_EXISTS, result.Errors[0]);
    }

    [Fact]
    public async Task Should_Create_Producto_When_Data_Is_Valid()
    {
        var request = new CreateProductoRequestDto
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

        _validatorMock.Setup(v => v.Validate(request)).Returns(new ValidationResult());

        _productoRepositoryMock.Setup(r => r.ProductoNombreActivoExistsAsync(request.Nombre, default))
                              .ReturnsAsync(false);

        _codigoBarraRepositoryMock.Setup(r => r.CodigoBarraActivoExistsAsync(It.IsAny<string>(), default))
                                 .ReturnsAsync(false);

        _productoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Producto>(), default))
                              .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new SaveResult
                       {
                           IsSuccess = true,
                           RowsAffected = 1,
                           ErrorMessage = null
                       });

        var result = await _useCase.ExecuteAsync(request, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.Created, result.HttpStatusCode);
        Assert.NotNull(result.Payload);
        Assert.Equal(request.Nombre, result.Payload.Nombre);

        _productoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Producto>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(default), Times.Once);
    }
}