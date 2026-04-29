using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Catalog.Domain.Entities;
using Turnify.Shared.Kernel.Abstractions;
using StaffEntity = Turnify.Modules.Catalog.Domain.Entities.Staff;

namespace Turnify.Modules.Catalog.Application.Abstractions;

public interface ICatalogDbContext : IUnitOfWork
{
    DbSet<Location> Locations { get; }
    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<Service> Services { get; }
    DbSet<StaffEntity> Staff { get; }
    DbSet<StaffService> StaffServices { get; }
    DbSet<StaffLocation> StaffLocations { get; }
}
