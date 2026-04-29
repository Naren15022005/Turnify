using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Turnify.Modules.Tenants.Application.Tenants.Commands.RegisterTenant;
using Turnify.Modules.Tenants.Infrastructure.Persistence;

namespace Turnify.Modules.Tenants.Infrastructure;

public static class TenantsModule
{
    public static IServiceCollection AddTenantsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<TenantsDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("Default"),
                new MySqlServerVersion(new Version(8, 0, 0)),
                mysql => mysql.MigrationsHistoryTable("__ef_migrations_tenants", "tenants")));

        services.AddScoped<ITenantsDbContext>(sp => sp.GetRequiredService<TenantsDbContext>());

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(RegisterTenantHandler).Assembly));

        return services;
    }
}
