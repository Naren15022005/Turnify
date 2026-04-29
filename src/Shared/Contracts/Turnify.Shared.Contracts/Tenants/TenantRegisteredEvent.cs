using MediatR;

namespace Turnify.Shared.Contracts.Tenants;

public sealed record TenantRegisteredEvent(
    long TenantId,
    string Slug,
    long OwnerUserId,
    DateTime OccurredAt) : INotification;
