using Turnify.Shared.Kernel.Abstractions;

namespace Turnify.Shared.Kernel.Domain;

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
