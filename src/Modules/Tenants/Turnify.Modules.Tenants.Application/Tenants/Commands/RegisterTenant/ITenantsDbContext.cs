using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Tenants.Domain.Entities;
using Turnify.Shared.Kernel.Abstractions;

namespace Turnify.Modules.Tenants.Application.Tenants.Commands.RegisterTenant;

public interface ITenantsDbContext : IUnitOfWork
{
    DbSet<Tenant> Tenants { get; }
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }
}
