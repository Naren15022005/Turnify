using MediatR;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Modules.Catalog.Domain.Entities;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Locations.Commands.CreateLocation;

public sealed class CreateLocationHandler(ICatalogDbContext db)
    : IRequestHandler<CreateLocationCommand, Result<LocationDto>>
{
    public async Task<Result<LocationDto>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = Location.Create(request.TenantId, request.Name,
            request.Address, request.City, request.Department, request.Phone);

        db.Locations.Add(location);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(location);
    }

    internal static LocationDto ToDto(Location l) =>
        new(l.Id, l.PublicId, l.Name, l.Address, l.City, l.Department, l.Phone, l.IsActive);
}
