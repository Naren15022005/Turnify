using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Tenants.Application.Tenants.Commands.RegisterTenant;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Tenants.Application.Tenants.Queries.GetTenant;

public sealed class GetTenantHandler(ITenantsDbContext db)
    : IRequestHandler<GetTenantQuery, Result<TenantDto>>
{
    public async Task<Result<TenantDto>> Handle(GetTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await db.Tenants
            .AsNoTracking()
            .Where(t => t.PublicId == request.PublicId && !t.IsDeleted)
            .Select(t => new TenantDto(
                t.Id, t.PublicId, t.Slug, t.Name,
                t.Timezone, t.Currency, t.Status.ToString()))
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant is null)
            return Result.Failure<TenantDto>(Error.NotFound("Tenant", request.PublicId));

        return tenant;
    }
}
