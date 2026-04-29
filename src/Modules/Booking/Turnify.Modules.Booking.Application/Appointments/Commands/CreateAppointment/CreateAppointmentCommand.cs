using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Booking.Application.Abstractions;
using Turnify.Modules.Booking.Domain.Entities;
using Turnify.Modules.Booking.Domain.Enums;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Booking.Application.Appointments.Commands.CreateAppointment;

public sealed record AppointmentDto(
    string PublicId,
    long StaffId,
    long ServiceId,
    string CustomerName,
    DateTime StartsAt,
    DateTime EndsAt,
    int DurationMinutes,
    AppointmentStatus Status);

public sealed record CreateAppointmentCommand(
    long TenantId,
    long StaffId,
    long ServiceId,
    long? LocationId,
    string CustomerName,
    string? CustomerEmail,
    string? CustomerPhone,
    DateTime StartsAt,
    string? Notes = null,
    long? CustomerId = null) : IRequest<Result<AppointmentDto>>;

public sealed class CreateAppointmentHandler(
    IBookingDbContext db,
    ICatalogReadContext catalog)
    : IRequestHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var service = await catalog.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId && s.TenantId == request.TenantId, cancellationToken);

        if (service is null)
            return Result.Failure<AppointmentDto>(Error.NotFound("Service", request.ServiceId));

        var staff = await catalog.Staff
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.StaffId && s.TenantId == request.TenantId, cancellationToken);

        if (staff is null)
            return Result.Failure<AppointmentDto>(Error.NotFound("Staff", request.StaffId));

        if (!staff.IsBookable)
            return Result.Failure<AppointmentDto>(Error.Validation("Staff.NotBookable", "This staff member is not available for bookings."));

        int duration = service.DurationMinutes + service.BufferAfterMinutes;
        var startsAt = request.StartsAt.ToUniversalTime();
        var endsAt = startsAt.AddMinutes(duration);

        // Concurrency: check for overlapping confirmed/in-progress appointments
        var overlap = await db.Appointments
            .AnyAsync(a =>
                a.StaffId == request.StaffId &&
                a.TenantId == request.TenantId &&
                a.Status != AppointmentStatus.CancelledByCustomer &&
                a.Status != AppointmentStatus.CancelledByBusiness &&
                a.Status != AppointmentStatus.NoShow &&
                a.StartsAt < endsAt &&
                a.EndsAt > startsAt,
                cancellationToken);

        if (overlap)
            return Result.Failure<AppointmentDto>(Error.Conflict("Appointment.Overlap",
                "The requested time slot overlaps with an existing appointment."));

        var appointment = Appointment.Create(
            request.TenantId, request.StaffId, request.ServiceId, request.LocationId,
            request.CustomerName, request.CustomerEmail, request.CustomerPhone,
            startsAt, duration, request.Notes, request.CustomerId);

        db.Appointments.Add(appointment);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(appointment);
    }

    internal static AppointmentDto ToDto(Appointment a) =>
        new(a.PublicId, a.StaffId, a.ServiceId, a.CustomerName, a.StartsAt, a.EndsAt, a.DurationMinutes, a.Status);
}
