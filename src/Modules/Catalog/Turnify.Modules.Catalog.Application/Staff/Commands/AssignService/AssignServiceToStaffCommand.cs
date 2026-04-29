using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Modules.Catalog.Domain.Entities;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Staff.Commands.AssignService;

public sealed record AssignServiceToStaffCommand(
    long TenantId,
    string StaffPublicId,
    string ServicePublicId) : IRequest<Result>;

public sealed class AssignServiceToStaffHandler(ICatalogDbContext db)
    : IRequestHandler<AssignServiceToStaffCommand, Result>
{
    public async Task<Result> Handle(AssignServiceToStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await db.Staff
            .FirstOrDefaultAsync(s => s.PublicId == request.StaffPublicId && s.TenantId == request.TenantId, cancellationToken);

        if (staff is null)
            return Result.Failure(Error.NotFound("Staff", request.StaffPublicId));

        var service = await db.Services
            .FirstOrDefaultAsync(s => s.PublicId == request.ServicePublicId && s.TenantId == request.TenantId, cancellationToken);

        if (service is null)
            return Result.Failure(Error.NotFound("Service", request.ServicePublicId));

        var already = await db.StaffServices
            .AnyAsync(ss => ss.StaffId == staff.Id && ss.ServiceId == service.Id, cancellationToken);

        if (already)
            return Result.Failure(Error.Conflict("StaffService.AlreadyAssigned", "Service is already assigned to this staff member."));

        db.StaffServices.Add(StaffService.Create(request.TenantId, staff.Id, service.Id));
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
