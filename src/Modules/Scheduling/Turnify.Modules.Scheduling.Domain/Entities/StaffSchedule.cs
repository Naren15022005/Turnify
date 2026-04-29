using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Scheduling.Domain.Entities;

public sealed class StaffSchedule : Entity<long>
{
    public long TenantId { get; private set; }
    public long StaffId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool IsActive { get; private set; }

    private StaffSchedule() { }

    public static StaffSchedule Create(long tenantId, long staffId, DayOfWeek dayOfWeek,
        TimeOnly startTime, TimeOnly endTime)
        => new()
        {
            TenantId = tenantId,
            StaffId = staffId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    public void Update(TimeOnly startTime, TimeOnly endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
    public void Activate()   { IsActive = true;  UpdatedAt = DateTime.UtcNow; }
}
