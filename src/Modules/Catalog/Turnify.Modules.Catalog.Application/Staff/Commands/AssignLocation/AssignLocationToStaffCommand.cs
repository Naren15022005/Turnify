using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Modules.Catalog.Domain.Entities;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Staff.Commands.AssignLocation;

public sealed record AssignLocationToStaffCommand(
    long TenantId,
    string StaffPublicId,
    string LocationPublicId) : IRequest<Result>;

public sealed class AssignLocationToStaffHandler(ICatalogDbContext db)
    : IRequestHandler<AssignLocationToStaffCommand, Result>
{
    public async Task<Result> Handle(AssignLocationToStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await db.Staff
            .FirstOrDefaultAsync(s => s.PublicId == request.StaffPublicId && s.TenantId == request.TenantId, cancellationToken);

        if (staff is null)
            return Result.Failure(Error.NotFound("Staff", request.StaffPublicId));

        var location = await db.Locations
            .FirstOrDefaultAsync(l => l.PublicId == request.LocationPublicId && l.TenantId == request.TenantId, cancellationToken);

        if (location is null)
            return Result.Failure(Error.NotFound("Location", request.LocationPublicId));

        var already = await db.StaffLocations
            .AnyAsync(sl => sl.StaffId == staff.Id && sl.LocationId == location.Id, cancellationToken);

        if (already)
            return Result.Failure(Error.Conflict("StaffLocation.AlreadyAssigned", "Location is already assigned to this staff member."));

        db.StaffLocations.Add(StaffLocation.Create(request.TenantId, staff.Id, location.Id));
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
