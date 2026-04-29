using MediatR;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Tenants.Application.Tenants.Commands.RegisterTenant;

public sealed record RegisterTenantCommand(
    string Name,
    string Slug,
    long OwnerUserId,
    string? Nit = null) : IRequest<Result<RegisterTenantResponse>>;

public sealed record RegisterTenantResponse(long TenantId, string PublicId, string Slug);
