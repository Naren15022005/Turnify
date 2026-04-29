using MediatR;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Tenants.Application.Tenants.Queries.GetTenant;

public sealed record GetTenantQuery(string PublicId) : IRequest<Result<TenantDto>>;

public sealed record TenantDto(
    long Id,
    string PublicId,
    string Slug,
    string Name,
    string Timezone,
    string Currency,
    string Status);
