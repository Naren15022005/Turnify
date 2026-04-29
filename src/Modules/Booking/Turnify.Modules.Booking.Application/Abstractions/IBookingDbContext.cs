using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Booking.Domain.Entities;
using Turnify.Shared.Kernel.Abstractions;

namespace Turnify.Modules.Booking.Application.Abstractions;

public interface IBookingDbContext : IUnitOfWork
{
    DbSet<Appointment> Appointments { get; }
}
