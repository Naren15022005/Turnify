using FluentValidation;

namespace Turnify.Modules.Tenants.Application.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(60)
            .Matches("^[a-z0-9-]+$").WithMessage("El slug solo puede contener letras minúsculas, números y guiones.");
        RuleFor(x => x.OwnerUserId).GreaterThan(0);
    }
}
