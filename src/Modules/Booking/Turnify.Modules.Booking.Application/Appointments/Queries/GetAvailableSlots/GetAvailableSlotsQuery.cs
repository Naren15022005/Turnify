using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Booking.Application.Abstractions;
using Turnify.Modules.Booking.Domain.Enums;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Booking.Application.Appointments.Queries.GetAvailableSlots;

public sealed record TimeSlotDto(DateTime StartsAt, DateTime EndsAt);

public sealed record GetAvailableSlotsQuery(
    long TenantId,
    long StaffId,
    long ServiceId,
    DateOnly Date,
    int SlotIntervalMinutes = 15) : IRequest<Result<List<TimeSlotDto>>>;

public sealed class GetAvailableSlotsHandler(
    IBookingDbContext db,
    ISchedulingReadContext scheduling,
    ICatalogReadContext catalog)
    : IRequestHandler<GetAvailableSlotsQuery, Result<List<TimeSlotDto>>>
{
    public async Task<Result<List<TimeSlotDto>>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
    {
        // 1. Get service duration
        var service = await catalog.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId && s.TenantId == request.TenantId, cancellationToken);

        if (service is null)
            return Result.Failure<List<TimeSlotDto>>(Error.NotFound("Service", request.ServiceId));

        int totalDuration = service.DurationMinutes + service.BufferAfterMinutes;

        // 2. Get staff recurring schedule for the day
        var daySchedule = await scheduling.StaffSchedules
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.StaffId == request.StaffId &&
                s.TenantId == request.TenantId &&
                s.DayOfWeek == request.Date.DayOfWeek &&
                s.IsActive,
                cancellationToken);

        if (daySchedule is null)
            return new List<TimeSlotDto>();

        // 3. Check if date is a holiday
        var isHoliday = await scheduling.Holidays
            .AsNoTracking()
            .AnyAsync(h =>
                h.TenantId == request.TenantId &&
                (h.Date == request.Date || (h.IsRecurring && h.Date.Month == request.Date.Month && h.Date.Day == request.Date.Day)),
                cancellationToken);

        if (isHoliday)
            return new List<TimeSlotDto>();

        // 4. Build day boundaries in UTC (simplified — using UTC dates directly)
        var dayStart = request.Date.ToDateTime(daySchedule.StartTime, DateTimeKind.Utc);
        var dayEnd   = request.Date.ToDateTime(daySchedule.EndTime,   DateTimeKind.Utc);

        // 5. Get time-off blocks that overlap this day
        var timeOffBlocks = await scheduling.StaffTimeOffs
            .AsNoTracking()
            .Where(t =>
                t.StaffId == request.StaffId &&
                t.TenantId == request.TenantId &&
                t.StartsAt < dayEnd && t.EndsAt > dayStart)
            .Select(t => new { t.StartsAt, t.EndsAt })
            .ToListAsync(cancellationToken);

        // 6. Get existing appointments for the day
        var existingAppointments = await db.Appointments
            .AsNoTracking()
            .Where(a =>
                a.StaffId == request.StaffId &&
                a.TenantId == request.TenantId &&
                a.Status != AppointmentStatus.CancelledByCustomer &&
                a.Status != AppointmentStatus.CancelledByBusiness &&
                a.Status != AppointmentStatus.NoShow &&
                a.StartsAt < dayEnd && a.EndsAt > dayStart)
            .Select(a => new { a.StartsAt, a.EndsAt })
            .ToListAsync(cancellationToken);

        // 7. Generate discrete slots
        var slots = new List<TimeSlotDto>();
        var cursor = dayStart;

        while (cursor.AddMinutes(totalDuration) <= dayEnd)
        {
            var slotEnd = cursor.AddMinutes(totalDuration);

            bool blocked =
                timeOffBlocks.Any(t => t.StartsAt < slotEnd && t.EndsAt > cursor) ||
                existingAppointments.Any(a => a.StartsAt < slotEnd && a.EndsAt > cursor);

            if (!blocked)
                slots.Add(new TimeSlotDto(cursor, slotEnd));

            cursor = cursor.AddMinutes(request.SlotIntervalMinutes);
        }

        return slots;
    }
}
