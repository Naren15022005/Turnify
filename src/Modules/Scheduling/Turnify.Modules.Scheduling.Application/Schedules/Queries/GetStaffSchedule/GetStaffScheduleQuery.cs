using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Scheduling.Application.Abstractions;
using Turnify.Modules.Scheduling.Application.Schedules.Commands.SetStaffSchedule;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Scheduling.Application.Schedules.Queries.GetStaffSchedule;

public sealed record GetStaffScheduleQuery(long TenantId, long StaffId) : IRequest<Result<List<DayScheduleDto>>>;

public sealed class GetStaffScheduleHandler(ISchedulingDbContext db)
    : IRequestHandler<GetStaffScheduleQuery, Result<List<DayScheduleDto>>>
{
    public async Task<Result<List<DayScheduleDto>>> Handle(GetStaffScheduleQuery request, CancellationToken cancellationToken)
    {
        var slots = await db.StaffSchedules
            .AsNoTracking()
            .Where(s => s.StaffId == request.StaffId && s.TenantId == request.TenantId && s.IsActive)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(cancellationToken);

        return slots.Select(s => new DayScheduleDto(s.DayOfWeek, s.StartTime, s.EndTime)).ToList();
    }
}
