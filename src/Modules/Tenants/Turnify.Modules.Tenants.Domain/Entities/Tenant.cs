using Turnify.Modules.Tenants.Domain.Events;
using Turnify.Shared.Kernel.Common;
using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Tenants.Domain.Entities;

public sealed class Tenant : AggregateRoot<long>
{
    public string PublicId { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Nit { get; private set; }
    public string Timezone { get; private set; } = "America/Bogota";
    public string Currency { get; private set; } = "COP";
    public TenantStatus Status { get; private set; }
    public long OwnerUserId { get; private set; }

    private Tenant() { }

    public static Tenant Create(string name, string slug, long ownerUserId, string? nit = null)
    {
        var tenant = new Tenant
        {
            PublicId = NewUlid.Generate(),
            Name = name,
            Slug = slug.ToLowerInvariant().Trim(),
            OwnerUserId = ownerUserId,
            Nit = nit,
            Status = TenantStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        tenant.Raise(new TenantCreatedEvent(tenant.PublicId, tenant.Slug, ownerUserId));
        return tenant;
    }

    public void Suspend() { Status = TenantStatus.Suspended; UpdatedAt = DateTime.UtcNow; }
    public void Activate() { Status = TenantStatus.Active; UpdatedAt = DateTime.UtcNow; }
}

public enum TenantStatus { Active, Suspended, Cancelled }
