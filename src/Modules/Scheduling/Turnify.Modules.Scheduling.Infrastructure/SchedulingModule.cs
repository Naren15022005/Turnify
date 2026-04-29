using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Turnify.Modules.Scheduling.Application.Abstractions;
using Turnify.Modules.Scheduling.Application.Schedules.Commands.SetStaffSchedule;
using Turnify.Modules.Scheduling.Infrastructure.Persistence;

namespace Turnify.Modules.Scheduling.Infrastructure;

public static class SchedulingModule
{
    public static IServiceCollection AddSchedulingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SchedulingDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("Default"),
                new MySqlServerVersion(new Version(8, 0, 0)),
                mysql => mysql.MigrationsHistoryTable("__ef_migrations_scheduling", "scheduling")));

        services.AddScoped<ISchedulingDbContext>(sp => sp.GetRequiredService<SchedulingDbContext>());

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(SetStaffScheduleHandler).Assembly));

        return services;
    }
}
