using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Shared.Kernel.Abstractions;
using Turnify.Shared.Kernel.Domain;

namespace Turnify.Shared.Infrastructure.Persistence;

public abstract class TurnifyDbContext(DbContextOptions options, IPublisher publisher) : DbContext(options)
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditFields();
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(cancellationToken);
        return result;
    }

    private void StampAuditFields()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<Entity<long>>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.GetType().GetProperty(nameof(Entity<long>.CreatedAt))!
                    .SetValue(entry.Entity, now);
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.GetType().GetProperty(nameof(Entity<long>.UpdatedAt))!
                    .SetValue(entry.Entity, now);
            }
        }
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregates = ChangeTracker
            .Entries<AggregateRoot<long>>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        aggregates.ForEach(a => a.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, cancellationToken);
    }
}
