using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Catalog.Domain.Entities;

public sealed class StaffService : Entity<long>
{
    public long TenantId { get; private set; }
    public long StaffId { get; private set; }
    public long ServiceId { get; private set; }

    public Staff Staff { get; private set; } = default!;
    public Service Service { get; private set; } = default!;

    private StaffService() { }

    public static StaffService Create(long tenantId, long staffId, long serviceId)
        => new()
        {
            TenantId = tenantId,
            StaffId = staffId,
            ServiceId = serviceId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
}
