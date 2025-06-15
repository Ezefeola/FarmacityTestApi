using Core.Contracts.DTOs.Productos.Request;
using Core.Domain.Entities;
using Core.Utilities.Validations;
using FluentValidation;

namespace Core.UseCases.Productos.Validators;
public class UpdateProductoValidator : AbstractValidator<UpdateProductoRequestDto>
{
    public UpdateProductoValidator()
    {
        When(x => x.Nombre is not null, () =>
        {
            RuleFor(x => x.Nombre!)
                .NotEmpty()
                    .WithMessage(ValidationMessages.NOT_EMPTY)
                .MaximumLength(Producto.Rules.NOMBRE_MAX_LENGTH)
                    .WithMessage(ValidationMessages.MAX_LENGTH);
        });

        When(x => x.Precio is not null, () =>
        {
            RuleFor(x => x.Precio!.Value)
                .GreaterThan(0)
                    .WithMessage(ValidationMessages.GREATER_THAN_ZERO);
        });

        When(x => x.CantidadEnStock is not null, () =>
        {
            RuleFor(x => x.CantidadEnStock!.Value)
                .GreaterThanOrEqualTo(0)
                    .WithMessage(ValidationMessages.Producto.CANTIDAD_DE_STOCK_NOT_NEGATIVE);
        });

        When(x => x.CodigosBarras.Count > 0, () =>
        {
            RuleForEach(x => x.CodigosBarras)
                .ChildRules(codigo =>
                {
                    codigo.RuleFor(c => c.Codigo)
                        .NotEmpty()
                            .WithMessage(ValidationMessages.NOT_EMPTY)
                        .MaximumLength(CodigoBarra.Rules.CODIGO_MAX_LENGTH)
                            .WithMessage(ValidationMessages.MAX_LENGTH);
                });
        });
    }
}