using Turnify.Shared.Kernel.Common;
using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Catalog.Domain.Entities;

public sealed class Staff : AggregateRoot<long>
{
    public long TenantId { get; private set; }
    public string PublicId { get; private set; } = default!;
    public long? UserId { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? ProfessionalTitle { get; private set; }
    public string? Bio { get; private set; }
    public string? PhotoUrl { get; private set; }
    public bool IsBookable { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<StaffService> _staffServices = [];
    private readonly List<StaffLocation> _staffLocations = [];
    public IReadOnlyList<StaffService> StaffServices => _staffServices.AsReadOnly();
    public IReadOnlyList<StaffLocation> StaffLocations => _staffLocations.AsReadOnly();

    private Staff() { }

    public static Staff Create(long tenantId, string firstName, string lastName,
        string? email = null, string? phone = null, string? professionalTitle = null,
        long? userId = null)
        => new()
        {
            TenantId = tenantId,
            PublicId = NewUlid.Generate(),
            UserId = userId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email?.Trim().ToLowerInvariant(),
            Phone = phone?.Trim(),
            ProfessionalTitle = professionalTitle?.Trim(),
            IsBookable = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    public void Update(string firstName, string lastName, string? email, string? phone,
        string? professionalTitle, string? bio, string? photoUrl, bool isBookable)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email?.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        ProfessionalTitle = professionalTitle?.Trim();
        Bio = bio?.Trim();
        PhotoUrl = photoUrl;
        IsBookable = isBookable;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
}
