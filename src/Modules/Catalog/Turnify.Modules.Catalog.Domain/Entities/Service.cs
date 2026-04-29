using Turnify.Shared.Kernel.Common;
using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Catalog.Domain.Entities;

public sealed class Service : AggregateRoot<long>
{
    public long TenantId { get; private set; }
    public string PublicId { get; private set; } = default!;
    public long? CategoryId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public int DurationMinutes { get; private set; }
    public int BufferBeforeMinutes { get; private set; }
    public int BufferAfterMinutes { get; private set; }
    public decimal Price { get; private set; }
    public string? ColorHex { get; private set; }
    public bool RequiresDeposit { get; private set; }
    public decimal? DepositAmount { get; private set; }
    public bool IsActive { get; private set; }

    public ServiceCategory? Category { get; private set; }
    private readonly List<StaffService> _staffServices = [];
    public IReadOnlyList<StaffService> StaffServices => _staffServices.AsReadOnly();

    private Service() { }

    public static Service Create(
        long tenantId,
        string name,
        int durationMinutes,
        decimal price,
        long? categoryId = null,
        string? description = null,
        int bufferBeforeMinutes = 0,
        int bufferAfterMinutes = 0,
        string? colorHex = null,
        bool requiresDeposit = false,
        decimal? depositAmount = null)
        => new()
        {
            TenantId = tenantId,
            PublicId = NewUlid.Generate(),
            CategoryId = categoryId,
            Name = name.Trim(),
            Description = description?.Trim(),
            DurationMinutes = durationMinutes,
            BufferBeforeMinutes = bufferBeforeMinutes,
            BufferAfterMinutes = bufferAfterMinutes,
            Price = price,
            ColorHex = colorHex,
            RequiresDeposit = requiresDeposit,
            DepositAmount = depositAmount,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    public void Update(string name, int durationMinutes, decimal price, string? description,
        int bufferBefore, int bufferAfter, string? colorHex, bool requiresDeposit, decimal? depositAmount)
    {
        Name = name.Trim();
        Description = description?.Trim();
        DurationMinutes = durationMinutes;
        BufferBeforeMinutes = bufferBefore;
        BufferAfterMinutes = bufferAfter;
        Price = price;
        ColorHex = colorHex;
        RequiresDeposit = requiresDeposit;
        DepositAmount = depositAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
}
