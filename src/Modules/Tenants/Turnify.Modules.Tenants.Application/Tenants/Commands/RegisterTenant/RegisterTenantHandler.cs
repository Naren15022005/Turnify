using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Tenants.Domain.Entities;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Tenants.Application.Tenants.Commands.RegisterTenant;

public sealed class RegisterTenantHandler(ITenantsDbContext db)
    : IRequestHandler<RegisterTenantCommand, Result<RegisterTenantResponse>>
{
    public async Task<Result<RegisterTenantResponse>> Handle(
        RegisterTenantCommand request,
        CancellationToken cancellationToken)
    {
        var slugTaken = await db.Tenants
            .AnyAsync(t => t.Slug == request.Slug.ToLowerInvariant(), cancellationToken);

        if (slugTaken)
            return Result.Failure<RegisterTenantResponse>(
                Error.Conflict("Tenant.SlugTaken", $"El slug '{request.Slug}' ya está en uso."));

        var tenant = Tenant.Create(request.Name, request.Slug, request.OwnerUserId, request.Nit);
        db.Tenants.Add(tenant);
        await db.SaveChangesAsync(cancellationToken);

        return new RegisterTenantResponse(tenant.Id, tenant.PublicId, tenant.Slug);
    }
}
