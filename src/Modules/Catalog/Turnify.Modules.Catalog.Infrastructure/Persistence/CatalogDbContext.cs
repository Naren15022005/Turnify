using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Modules.Catalog.Domain.Entities;
using Turnify.Modules.Catalog.Infrastructure.Persistence.Configurations;
using Turnify.Shared.Infrastructure.Persistence;

namespace Turnify.Modules.Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options, IPublisher publisher)
    : TurnifyDbContext(options, publisher), ICatalogDbContext
{
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<StaffService> StaffServices => Set<StaffService>();
    public DbSet<StaffLocation> StaffLocations => Set<StaffLocation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
        modelBuilder.ApplyConfiguration(new ServiceCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ServiceConfiguration());
        modelBuilder.ApplyConfiguration(new StaffConfiguration());
        modelBuilder.ApplyConfiguration(new StaffServiceConfiguration());
        modelBuilder.ApplyConfiguration(new StaffLocationConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
