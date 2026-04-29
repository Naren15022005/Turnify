using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Modules.Catalog.Application.Locations.Commands.CreateLocation;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Locations.Queries.GetLocations;

public sealed record GetLocationsQuery(long TenantId) : IRequest<Result<List<LocationDto>>>;

public sealed class GetLocationsHandler(ICatalogDbContext db)
    : IRequestHandler<GetLocationsQuery, Result<List<LocationDto>>>
{
    public async Task<Result<List<LocationDto>>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var locations = await db.Locations
            .AsNoTracking()
            .Where(l => l.TenantId == request.TenantId && !l.IsDeleted)
            .OrderBy(l => l.Name)
            .Select(l => new LocationDto(l.Id, l.PublicId, l.Name, l.Address, l.City, l.Department, l.Phone, l.IsActive))
            .ToListAsync(cancellationToken);

        return locations;
    }
}
