using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Catalog.Domain.Entities;

namespace Turnify.Modules.Booking.Application.Abstractions;

public interface ICatalogReadContext
{
    DbSet<Service> Services { get; }
    DbSet<Staff> Staff { get; }
}
