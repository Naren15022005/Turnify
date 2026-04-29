using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Scheduling.Application.Abstractions;
using Turnify.Modules.Scheduling.Domain.Entities;
using Turnify.Modules.Scheduling.Infrastructure.Persistence.Configurations;
using Turnify.Shared.Infrastructure.Persistence;

namespace Turnify.Modules.Scheduling.Infrastructure.Persistence;

public sealed class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options, IPublisher publisher)
    : TurnifyDbContext(options, publisher), ISchedulingDbContext
{
    public DbSet<StaffSchedule> StaffSchedules => Set<StaffSchedule>();
    public DbSet<StaffTimeOff> StaffTimeOffs => Set<StaffTimeOff>();
    public DbSet<Holiday> Holidays => Set<Holiday>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("scheduling");
        modelBuilder.ApplyConfiguration(new StaffScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new StaffTimeOffConfiguration());
        modelBuilder.ApplyConfiguration(new HolidayConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
