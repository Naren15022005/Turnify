using Turnify.Modules.Identity.Domain.Events;
using Turnify.Shared.Kernel.Common;
using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Identity.Domain.Entities;

public sealed class User : AggregateRoot<long>
{
    public string PublicId { get; private set; } = default!;
    public long? TenantId { get; private set; }
    public string Email { get; private set; } = default!;
    public string EmailNormalized { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? Phone { get; private set; }
    public DateTime? EmailVerifiedAt { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    public UserStatus Status { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        long? tenantId = null,
        string? phone = null)
    {
        var user = new User
        {
            PublicId = NewUlid.Generate(),
            TenantId = tenantId,
            Email = email.Trim().ToLowerInvariant(),
            EmailNormalized = email.Trim().ToUpperInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Phone = phone,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.Raise(new UserCreatedEvent(user.PublicId, user.Email, tenantId));
        return user;
    }

    public bool IsLockedOut() => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            // Exponential backoff: 15min * 2^(attempts-5)
            var minutes = 15 * Math.Pow(2, FailedLoginAttempts - 5);
            LockedUntil = DateTime.UtcNow.AddMinutes(Math.Min(minutes, 1440));
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        EmailVerifiedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum UserStatus { Active, Locked, Disabled }
