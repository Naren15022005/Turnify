using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Identity.Domain.Entities;

public sealed class RefreshToken : Entity<long>
{
    public long UserId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public long? ReplacedById { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    public User User { get; private set; } = default!;

    private RefreshToken() { }

    public static RefreshToken Create(long userId, string tokenHash, string? ipAddress, string? userAgent)
        => new()
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    public void Revoke(long? replacedById = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedById = replacedById;
        UpdatedAt = DateTime.UtcNow;
    }
}
