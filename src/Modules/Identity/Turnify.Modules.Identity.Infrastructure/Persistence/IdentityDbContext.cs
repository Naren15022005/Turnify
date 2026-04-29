using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Identity.Application.Abstractions;
using Turnify.Modules.Identity.Domain.Entities;
using Turnify.Modules.Identity.Infrastructure.Persistence.Configurations;
using Turnify.Shared.Infrastructure.Persistence;

namespace Turnify.Modules.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options, IPublisher publisher)
    : TurnifyDbContext(options, publisher), IIdentityDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
