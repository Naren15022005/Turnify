using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Identity.Domain.Events;

public sealed record UserCreatedEvent(
    string PublicId,
    string Email,
    long? TenantId) : DomainEvent;
