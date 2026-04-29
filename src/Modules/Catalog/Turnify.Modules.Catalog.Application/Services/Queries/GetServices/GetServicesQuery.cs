using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Modules.Catalog.Application.Services.Commands.CreateService;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Services.Queries.GetServices;

public sealed record GetServicesQuery(long TenantId, bool OnlyActive = true) : IRequest<Result<List<ServiceDto>>>;

public sealed class GetServicesHandler(ICatalogDbContext db)
    : IRequestHandler<GetServicesQuery, Result<List<ServiceDto>>>
{
    public async Task<Result<List<ServiceDto>>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var query = db.Services
            .AsNoTracking()
            .Where(s => s.TenantId == request.TenantId && !s.IsDeleted);

        if (request.OnlyActive)
            query = query.Where(s => s.IsActive);

        var services = await query
            .OrderBy(s => s.Name)
            .Select(s => new ServiceDto(
                s.Id, s.PublicId, s.Name, s.Description,
                s.DurationMinutes, s.BufferBeforeMinutes, s.BufferAfterMinutes,
                s.Price, s.ColorHex, s.RequiresDeposit, s.DepositAmount,
                s.CategoryId, s.IsActive))
            .ToListAsync(cancellationToken);

        return services;
    }
}
