using MediatR;

namespace Turnify.Shared.Contracts.Identity;

public sealed record UserRegisteredEvent(
    long UserId,
    long? TenantId,
    string Email,
    DateTime OccurredAt) : INotification;
