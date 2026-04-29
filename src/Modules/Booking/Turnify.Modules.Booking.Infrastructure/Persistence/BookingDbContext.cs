using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Booking.Application.Abstractions;
using Turnify.Modules.Booking.Domain.Entities;
using Turnify.Modules.Booking.Infrastructure.Persistence.Configurations;
using Turnify.Shared.Infrastructure.Persistence;

namespace Turnify.Modules.Booking.Infrastructure.Persistence;

public sealed class BookingDbContext(DbContextOptions<BookingDbContext> options, IPublisher publisher)
    : TurnifyDbContext(options, publisher), IBookingDbContext
{
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("booking");
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
