using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Booking.Application.Abstractions;
using Turnify.Modules.Catalog.Domain.Entities;
using Turnify.Modules.Catalog.Infrastructure.Persistence;
using StaffEntity = Turnify.Modules.Catalog.Domain.Entities.Staff;

namespace Turnify.Modules.Booking.Infrastructure.Adapters;

internal sealed class CatalogReadAdapter(CatalogDbContext context) : ICatalogReadContext
{
    public DbSet<Service> Services => context.Services;
    public DbSet<StaffEntity> Staff => context.Staff;
}
