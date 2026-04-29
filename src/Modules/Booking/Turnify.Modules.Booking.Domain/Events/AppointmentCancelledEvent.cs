using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Booking.Domain.Events;

public sealed record AppointmentCancelledEvent(long AppointmentId, long TenantId, string CancelledBy)
    : DomainEvent;
