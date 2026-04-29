using FluentValidation;

namespace Turnify.Modules.Catalog.Application.Staff.Commands.CreateStaff;

public sealed class CreateStaffValidator : AbstractValidator<CreateStaffCommand>
{
    public CreateStaffValidator()
    {
        RuleFor(x => x.TenantId).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(80);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(150).When(x => x.Email is not null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
        RuleFor(x => x.ProfessionalTitle).MaximumLength(100).When(x => x.ProfessionalTitle is not null);
    }
}
