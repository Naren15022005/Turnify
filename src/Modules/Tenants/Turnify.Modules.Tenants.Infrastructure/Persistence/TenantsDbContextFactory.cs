using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MediatR;

namespace Turnify.Modules.Tenants.Infrastructure.Persistence;

public sealed class TenantsDbContextFactory : IDesignTimeDbContextFactory<TenantsDbContext>
{
    public TenantsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TenantsDbContext>()
            .UseMySql(
                "Server=localhost;Port=3306;Database=turnify;Uid=root;Pwd=root;",
                new MySqlServerVersion(new Version(8, 0, 0)))
            .Options;

        return new TenantsDbContext(options, new NoOpPublisher());
    }

    private sealed class NoOpPublisher : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification => Task.CompletedTask;
    }
}
