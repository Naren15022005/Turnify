using Turnify.Shared.Kernel.Common;
using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Catalog.Domain.Entities;

public sealed class Location : AggregateRoot<long>
{
    public long TenantId { get; private set; }
    public string PublicId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? Department { get; private set; }
    public string? Phone { get; private set; }
    public string Timezone { get; private set; } = "America/Bogota";
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public bool IsActive { get; private set; }

    private Location() { }

    public static Location Create(long tenantId, string name, string? address = null,
        string? city = null, string? department = null, string? phone = null)
        => new()
        {
            TenantId = tenantId,
            PublicId = NewUlid.Generate(),
            Name = name.Trim(),
            Address = address?.Trim(),
            City = city?.Trim(),
            Department = department?.Trim(),
            Phone = phone?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    public void Update(string name, string? address, string? city, string? department, string? phone)
    {
        Name = name.Trim();
        Address = address?.Trim();
        City = city?.Trim();
        Department = department?.Trim();
        Phone = phone?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
    public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
}
