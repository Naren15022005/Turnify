using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Modules.Catalog.Application.Locations.Commands.CreateLocation;
using Turnify.Modules.Catalog.Infrastructure.Persistence;

namespace Turnify.Modules.Catalog.Infrastructure;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("Default"),
                new MySqlServerVersion(new Version(8, 0, 0)),
                mysql => mysql.MigrationsHistoryTable("__ef_migrations_catalog", "catalog")));

        services.AddScoped<ICatalogDbContext>(sp => sp.GetRequiredService<CatalogDbContext>());

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateLocationHandler).Assembly));

        return services;
    }
}
