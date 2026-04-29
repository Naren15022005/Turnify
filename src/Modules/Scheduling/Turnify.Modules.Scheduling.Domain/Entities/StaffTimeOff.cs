using Turnify.Shared.Kernel.Common;
using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Scheduling.Domain.Entities;

public sealed class StaffTimeOff : AggregateRoot<long>
{
    public long TenantId { get; private set; }
    public string PublicId { get; private set; } = default!;
    public long StaffId { get; private set; }
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }
    public string? Reason { get; private set; }

    private StaffTimeOff() { }

    public static StaffTimeOff Create(long tenantId, long staffId,
        DateTime startsAt, DateTime endsAt, string? reason = null)
        => new()
        {
            TenantId = tenantId,
            PublicId = NewUlid.Generate(),
            StaffId = staffId,
            StartsAt = startsAt.ToUniversalTime(),
            EndsAt = endsAt.ToUniversalTime(),
            Reason = reason?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
}
