using Core.Contracts.DTOs.Productos.Request;
using Core.Domain.Entities;
using Core.Utilities.Validations;
using FluentValidation;

namespace Core.UseCases.Productos.Validators;
public class CreateProductValidator : AbstractValidator<CreateProductoRequestDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
                .WithMessage(ValidationMessages.NOT_EMPTY)
            .MaximumLength(Producto.Rules.NOMBRE_MAX_LENGTH)
                .WithMessage(ValidationMessages.MAX_LENGTH);

        RuleFor(x => x.Precio)
            .GreaterThan(0)
                .WithMessage(ValidationMessages.GREATER_THAN_ZERO);

        RuleFor(x => x.CantidadEnStock)
            .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationMessages.Producto.CANTIDAD_DE_STOCK_NOT_NEGATIVE);

        RuleFor(x => x.CodigosBarras)
            .NotNull()
                .WithMessage(ValidationMessages.NOT_EMPTY)
            .Must(list => list.Any())
                .WithMessage(ValidationMessages.NOT_EMPTY)
            .Must(list => list
                .Select(cb => cb.Codigo.Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() == list.Count)
            .WithMessage(ValidationMessages.Producto.CODIGO_BARRA_CANT_BE_DUPLICATED);

        RuleForEach(x => x.CodigosBarras)
            .ChildRules(codigo =>
            {
                codigo.RuleFor(c => c.Codigo)
                    .NotEmpty()
                        .WithMessage(ValidationMessages.NOT_EMPTY)
                    .MaximumLength(CodigoBarra.Rules.CODIGO_MAX_LENGTH)
                        .WithMessage(ValidationMessages.MAX_LENGTH);
            });
    }
}