﻿using Core.Contracts.DTOs.Productos.Request;
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

        When(x => x.CodigosBarras.Count > 0 && x.CodigosBarras is not null, () =>
        {
            RuleFor(x => x.CodigosBarras)
                .NotNull()
                    .WithMessage(ValidationMessages.NOT_EMPTY)
                .Must(codigosBarras => codigosBarras.Count > 0)
                    .WithMessage(ValidationMessages.NOT_EMPTY)
                .Must(codigosBarras =>
                {
                    List<string> validatedCodigosBarras = codigosBarras
                                                    .Where(x => !string.IsNullOrWhiteSpace(x?.Codigo))
                                                    .Select(x => x!.Codigo!.Trim().ToLowerInvariant())
                                                    .ToList();

                    bool isValid = validatedCodigosBarras.Distinct().Count() == validatedCodigosBarras.Count;
                    return isValid;
                })
                    .WithMessage(ValidationMessages.Producto.CODIGO_BARRA_CANT_BE_DUPLICATED);
        });

        RuleForEach(x => x.CodigosBarras)
            .ChildRules(codigo =>
            {
                codigo.RuleFor(x => x.Codigo)
                    .NotEmpty()
                        .WithMessage(ValidationMessages.NOT_EMPTY)
                    .MaximumLength(CodigoBarra.Rules.CODIGO_MAX_LENGTH)
                        .WithMessage(ValidationMessages.MAX_LENGTH);
            });
    }
}