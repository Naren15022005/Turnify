using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Scheduling.Application.Abstractions;
using Turnify.Modules.Scheduling.Domain.Entities;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Scheduling.Application.Schedules.Commands.SetStaffSchedule;

public sealed record DayScheduleDto(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime);

public sealed record SetStaffScheduleCommand(
    long TenantId,
    long StaffId,
    List<DayScheduleDto> Slots) : IRequest<Result>;

public sealed class SetStaffScheduleHandler(ISchedulingDbContext db)
    : IRequestHandler<SetStaffScheduleCommand, Result>
{
    public async Task<Result> Handle(SetStaffScheduleCommand request, CancellationToken cancellationToken)
    {
        var existing = await db.StaffSchedules
            .Where(s => s.StaffId == request.StaffId && s.TenantId == request.TenantId)
            .ToListAsync(cancellationToken);

        // deactivate all existing slots not in the new set
        var newDays = request.Slots.Select(s => s.Day).ToHashSet();
        foreach (var schedule in existing.Where(s => !newDays.Contains(s.DayOfWeek)))
            schedule.Deactivate();

        foreach (var slot in request.Slots)
        {
            var current = existing.FirstOrDefault(s => s.DayOfWeek == slot.Day);
            if (current is null)
                db.StaffSchedules.Add(StaffSchedule.Create(request.TenantId, request.StaffId,
                    slot.Day, slot.StartTime, slot.EndTime));
            else
            {
                current.Activate();
                current.Update(slot.StartTime, slot.EndTime);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
