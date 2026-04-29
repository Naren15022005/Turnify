using MediatR;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Staff.Commands.CreateStaff;

public sealed class CreateStaffHandler(ICatalogDbContext db)
    : IRequestHandler<CreateStaffCommand, Result<StaffDto>>
{
    public async Task<Result<StaffDto>> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = Domain.Entities.Staff.Create(
            request.TenantId, request.FirstName, request.LastName,
            request.Email, request.Phone, request.ProfessionalTitle, request.UserId);

        db.Staff.Add(staff);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(staff);
    }

    internal static StaffDto ToDto(Domain.Entities.Staff s) => new(
        s.Id, s.PublicId, s.FirstName, s.LastName,
        s.Email, s.Phone, s.ProfessionalTitle, s.Bio, s.PhotoUrl,
        s.IsBookable, s.IsActive,
        s.StaffServices.Select(ss => ss.ServiceId).ToList(),
        s.StaffLocations.Select(sl => sl.LocationId).ToList());
}
