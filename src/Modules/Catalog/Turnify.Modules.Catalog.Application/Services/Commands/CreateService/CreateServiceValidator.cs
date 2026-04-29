using FluentValidation;

namespace Turnify.Modules.Catalog.Application.Services.Commands.CreateService;

public sealed class CreateServiceValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceValidator()
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DurationMinutes).InclusiveBetween(5, 480);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BufferBeforeMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BufferAfterMinutes).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ColorHex).Matches("^#[0-9A-Fa-f]{6}$")
            .When(x => x.ColorHex is not null).WithMessage("El color debe ser un HEX válido (#RRGGBB).");
        RuleFor(x => x.DepositAmount).GreaterThan(0)
            .When(x => x.RequiresDeposit).WithMessage("El monto del depósito debe ser mayor a 0 si se requiere depósito.");
    }
}
