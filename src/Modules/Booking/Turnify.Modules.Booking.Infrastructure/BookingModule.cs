using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Turnify.Modules.Booking.Application.Abstractions;
using Turnify.Modules.Booking.Application.Appointments.Commands.CreateAppointment;
using Turnify.Modules.Booking.Infrastructure.Adapters;
using Turnify.Modules.Booking.Infrastructure.Persistence;

namespace Turnify.Modules.Booking.Infrastructure;

public static class BookingModule
{
    public static IServiceCollection AddBookingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("Default"),
                new MySqlServerVersion(new Version(8, 0, 0)),
                mysql => mysql.MigrationsHistoryTable("__ef_migrations_booking", "booking")));

        services.AddScoped<IBookingDbContext>(sp => sp.GetRequiredService<BookingDbContext>());
        services.AddScoped<ISchedulingReadContext, SchedulingReadAdapter>();
        services.AddScoped<ICatalogReadContext, CatalogReadAdapter>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateAppointmentHandler).Assembly));

        return services;
    }
}
