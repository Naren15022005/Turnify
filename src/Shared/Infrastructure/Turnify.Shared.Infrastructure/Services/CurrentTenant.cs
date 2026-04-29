using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Turnify.Shared.Kernel.Abstractions;

namespace Turnify.Shared.Infrastructure.Services;

public sealed class CurrentTenant : ICurrentTenant
{
    public long? Id { get; }
    public string? Slug { get; }
    public bool IsAuthenticated { get; }

    public CurrentTenant(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;

        if (!IsAuthenticated) return;

        var tenantIdClaim = user!.FindFirst("tenant_id")?.Value;
        if (long.TryParse(tenantIdClaim, out var tenantId))
            Id = tenantId;

        Slug = user.FindFirst("tenant_slug")?.Value;
    }
}
