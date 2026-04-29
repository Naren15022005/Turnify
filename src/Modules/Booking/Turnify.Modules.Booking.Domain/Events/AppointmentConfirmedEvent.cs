using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Booking.Domain.Events;

public sealed record AppointmentConfirmedEvent(long AppointmentId, long TenantId, long StaffId, DateTime StartsAt)
    : DomainEvent;
