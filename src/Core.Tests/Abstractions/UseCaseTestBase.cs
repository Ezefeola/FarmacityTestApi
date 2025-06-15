using Core.Contracts.Repositories;
using Core.Contracts.UnitOfWork;
using FluentValidation;
using Moq;

namespace Core.Tests.Abstractions;
public abstract class UseCaseTestBase<TUseCase>
{
    protected readonly Mock<IUnitOfWork> UnitOfWorkMock;

    protected readonly Mock<IProductoRepository> ProductoRepositoryMock;
    protected readonly Mock<ICodigoBarraRepository> CodigoBarraRepositoryMock;

    protected TUseCase UseCase;

    protected UseCaseTestBase(Func<IUnitOfWork, TUseCase> useCaseFactory)
    {
        ProductoRepositoryMock = new Mock<IProductoRepository>();
        CodigoBarraRepositoryMock = new Mock<ICodigoBarraRepository>();

        UnitOfWorkMock = new Mock<IUnitOfWork>();
        UnitOfWorkMock.Setup(u => u.ProductoRepository).Returns(ProductoRepositoryMock.Object);
        UnitOfWorkMock.Setup(u => u.CodigoBarraRepository).Returns(CodigoBarraRepositoryMock.Object);

        UseCase = useCaseFactory(UnitOfWorkMock.Object);
    }
}

public abstract class UseCaseTestBase<TUseCase, TRequestDto> : UseCaseTestBase<TUseCase>
{
    protected readonly Mock<IValidator<TRequestDto>> ValidatorMock;

    protected UseCaseTestBase(Func<IUnitOfWork, IValidator<TRequestDto>, TUseCase> useCaseFactory)
        : base(unitOfWork => useCaseFactory(unitOfWork, new Mock<IValidator<TRequestDto>>().Object))
    {
        ValidatorMock = new Mock<IValidator<TRequestDto>>();
        UseCase = useCaseFactory(UnitOfWorkMock.Object, ValidatorMock.Object);
    }
}