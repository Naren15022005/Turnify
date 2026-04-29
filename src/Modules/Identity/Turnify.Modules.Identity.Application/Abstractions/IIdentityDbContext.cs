using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Identity.Domain.Entities;
using Turnify.Shared.Kernel.Abstractions;

namespace Turnify.Modules.Identity.Application.Abstractions;

public interface IIdentityDbContext : IUnitOfWork
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
}
