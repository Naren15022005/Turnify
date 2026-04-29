using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Booking.Application.Abstractions;
using Turnify.Modules.Scheduling.Domain.Entities;
using Turnify.Modules.Scheduling.Infrastructure.Persistence;

namespace Turnify.Modules.Booking.Infrastructure.Adapters;

internal sealed class SchedulingReadAdapter(SchedulingDbContext context) : ISchedulingReadContext
{
    public DbSet<StaffSchedule> StaffSchedules => context.StaffSchedules;
    public DbSet<StaffTimeOff> StaffTimeOffs => context.StaffTimeOffs;
    public DbSet<Holiday> Holidays => context.Holidays;
}
