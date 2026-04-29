using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Tenants.Domain.Events;

public sealed record TenantCreatedEvent(
    string PublicId,
    string Slug,
    long OwnerUserId) : DomainEvent;
