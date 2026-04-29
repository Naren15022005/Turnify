using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Booking.Application.Abstractions;
using Turnify.Modules.Booking.Application.Appointments.Commands.CreateAppointment;
using Turnify.Modules.Booking.Domain.Enums;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Booking.Application.Appointments.Queries.GetAppointments;

public sealed record GetAppointmentsQuery(
    long TenantId,
    long? StaffId = null,
    DateOnly? Date = null,
    AppointmentStatus? Status = null) : IRequest<Result<List<AppointmentDto>>>;

public sealed class GetAppointmentsHandler(IBookingDbContext db)
    : IRequestHandler<GetAppointmentsQuery, Result<List<AppointmentDto>>>
{
    public async Task<Result<List<AppointmentDto>>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Appointments
            .AsNoTracking()
            .Where(a => a.TenantId == request.TenantId);

        if (request.StaffId.HasValue)
            query = query.Where(a => a.StaffId == request.StaffId.Value);

        if (request.Date.HasValue)
        {
            var start = request.Date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end = start.AddDays(1);
            query = query.Where(a => a.StartsAt >= start && a.StartsAt < end);
        }

        if (request.Status.HasValue)
            query = query.Where(a => a.Status == request.Status.Value);

        var appointments = await query
            .OrderBy(a => a.StartsAt)
            .ToListAsync(cancellationToken);

        return appointments.Select(CreateAppointmentHandler.ToDto).ToList();
    }
}
