using MediatR;

namespace Turnify.Shared.Kernel.Abstractions;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
