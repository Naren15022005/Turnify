using Turnify.Modules.Booking.Domain.Enums;
using Turnify.Modules.Booking.Domain.Events;
using Turnify.Shared.Kernel.Common;
using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Booking.Domain.Entities;

public sealed class Appointment : AggregateRoot<long>
{
    public long TenantId { get; private set; }
    public string PublicId { get; private set; } = default!;
    public long StaffId { get; private set; }
    public long ServiceId { get; private set; }
    public long? LocationId { get; private set; }
    public long? CustomerId { get; private set; }

    public string CustomerName { get; private set; } = default!;
    public string? CustomerEmail { get; private set; }
    public string? CustomerPhone { get; private set; }

    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }
    public int DurationMinutes { get; private set; }

    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }

    private Appointment() { }

    public static Appointment Create(
        long tenantId, long staffId, long serviceId, long? locationId,
        string customerName, string? customerEmail, string? customerPhone,
        DateTime startsAt, int durationMinutes, string? notes = null, long? customerId = null)
        => new()
        {
            TenantId = tenantId,
            PublicId = NewUlid.Generate(),
            StaffId = staffId,
            ServiceId = serviceId,
            LocationId = locationId,
            CustomerId = customerId,
            CustomerName = customerName.Trim(),
            CustomerEmail = customerEmail?.Trim().ToLowerInvariant(),
            CustomerPhone = customerPhone?.Trim(),
            StartsAt = startsAt.ToUniversalTime(),
            EndsAt = startsAt.ToUniversalTime().AddMinutes(durationMinutes),
            DurationMinutes = durationMinutes,
            Status = AppointmentStatus.Confirmed,
            Notes = notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    public void Confirm()
    {
        if (Status != AppointmentStatus.PendingPayment)
            throw new InvalidOperationException("Only PendingPayment appointments can be confirmed.");
        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
        Raise(new AppointmentConfirmedEvent(Id, TenantId, StaffId, StartsAt));
    }

    public void Start()
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Only Confirmed appointments can be started.");
        Status = AppointmentStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.InProgress)
            throw new InvalidOperationException("Only InProgress appointments can be completed.");
        Status = AppointmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkNoShow()
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Only Confirmed appointments can be marked as no-show.");
        Status = AppointmentStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelByCustomer(string? reason = null)
    {
        if (Status is not (AppointmentStatus.PendingPayment or AppointmentStatus.Confirmed))
            throw new InvalidOperationException("Appointment cannot be cancelled at this stage.");
        Status = AppointmentStatus.CancelledByCustomer;
        CancellationReason = reason?.Trim();
        UpdatedAt = DateTime.UtcNow;
        Raise(new AppointmentCancelledEvent(Id, TenantId, "customer"));
    }

    public void CancelByBusiness(string? reason = null)
    {
        if (Status is not (AppointmentStatus.PendingPayment or AppointmentStatus.Confirmed))
            throw new InvalidOperationException("Appointment cannot be cancelled at this stage.");
        Status = AppointmentStatus.CancelledByBusiness;
        CancellationReason = reason?.Trim();
        UpdatedAt = DateTime.UtcNow;
        Raise(new AppointmentCancelledEvent(Id, TenantId, "business"));
    }
}
