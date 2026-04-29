using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Scheduling.Domain.Entities;
using Turnify.Shared.Kernel.Abstractions;

namespace Turnify.Modules.Scheduling.Application.Abstractions;

public interface ISchedulingDbContext : IUnitOfWork
{
    DbSet<StaffSchedule> StaffSchedules { get; }
    DbSet<StaffTimeOff> StaffTimeOffs { get; }
    DbSet<Holiday> Holidays { get; }
}
