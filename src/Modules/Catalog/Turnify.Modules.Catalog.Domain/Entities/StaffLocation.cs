using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Catalog.Domain.Entities;

public sealed class StaffLocation : Entity<long>
{
    public long TenantId { get; private set; }
    public long StaffId { get; private set; }
    public long LocationId { get; private set; }

    public Staff Staff { get; private set; } = default!;
    public Location Location { get; private set; } = default!;

    private StaffLocation() { }

    public static StaffLocation Create(long tenantId, long staffId, long locationId)
        => new()
        {
            TenantId = tenantId,
            StaffId = staffId,
            LocationId = locationId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
}
