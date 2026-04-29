using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Modules.Catalog.Application.Staff.Commands.CreateStaff;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Staff.Queries.GetStaff;

public sealed record GetStaffQuery(long TenantId, bool OnlyActive = true) : IRequest<Result<List<StaffDto>>>;

public sealed class GetStaffHandler(ICatalogDbContext db)
    : IRequestHandler<GetStaffQuery, Result<List<StaffDto>>>
{
    public async Task<Result<List<StaffDto>>> Handle(GetStaffQuery request, CancellationToken cancellationToken)
    {
        var query = db.Staff
            .AsNoTracking()
            .Include(s => s.StaffServices)
            .Include(s => s.StaffLocations)
            .Where(s => s.TenantId == request.TenantId && !s.IsDeleted);

        if (request.OnlyActive)
            query = query.Where(s => s.IsActive);

        var staff = await query
            .OrderBy(s => s.FirstName).ThenBy(s => s.LastName)
            .ToListAsync(cancellationToken);

        return staff.Select(CreateStaffHandler.ToDto).ToList();
    }
}
