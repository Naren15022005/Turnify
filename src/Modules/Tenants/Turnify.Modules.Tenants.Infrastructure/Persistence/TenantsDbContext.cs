using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Tenants.Application.Tenants.Commands.RegisterTenant;
using Turnify.Modules.Tenants.Domain.Entities;
using Turnify.Modules.Tenants.Infrastructure.Persistence.Configurations;
using Turnify.Shared.Infrastructure.Persistence;

namespace Turnify.Modules.Tenants.Infrastructure.Persistence;

public sealed class TenantsDbContext(DbContextOptions<TenantsDbContext> options, IPublisher publisher)
    : TurnifyDbContext(options, publisher), ITenantsDbContext
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tenants");
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionPlanConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
