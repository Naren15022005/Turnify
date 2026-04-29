using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Catalog.Domain.Entities;

public sealed class ServiceCategory : Entity<long>
{
    public long TenantId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private ServiceCategory() { }

    public static ServiceCategory Create(long tenantId, string name, string? description = null)
        => new()
        {
            TenantId = tenantId,
            Name = name.Trim(),
            Description = description?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    public void Update(string name, string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
