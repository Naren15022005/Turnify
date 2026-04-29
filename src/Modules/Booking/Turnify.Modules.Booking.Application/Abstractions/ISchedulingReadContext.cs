using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Scheduling.Domain.Entities;

namespace Turnify.Modules.Booking.Application.Abstractions;

public interface ISchedulingReadContext
{
    DbSet<StaffSchedule> StaffSchedules { get; }
    DbSet<StaffTimeOff> StaffTimeOffs { get; }
    DbSet<Holiday> Holidays { get; }
}
