using Turnify.Shared.Kernel.Common;
using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Scheduling.Domain.Entities;

public sealed class Holiday : AggregateRoot<long>
{
    public long TenantId { get; private set; }
    public string PublicId { get; private set; } = default!;
    public DateOnly Date { get; private set; }
    public string Name { get; private set; } = default!;
    /// <summary>If true, recurs on the same month+day every year.</summary>
    public bool IsRecurring { get; private set; }

    private Holiday() { }

    public static Holiday Create(long tenantId, DateOnly date, string name, bool isRecurring = false)
        => new()
        {
            TenantId = tenantId,
            PublicId = NewUlid.Generate(),
            Date = date,
            Name = name.Trim(),
            IsRecurring = isRecurring,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
}
