using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Identity.Domain.Entities;

namespace Turnify.Modules.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.PublicId).HasColumnType("char(26)").IsRequired();
        builder.HasIndex(u => u.PublicId).IsUnique();

        builder.Property(u => u.Email).HasMaxLength(150).IsRequired();
        builder.Property(u => u.EmailNormalized).HasMaxLength(150).IsRequired();
        builder.HasIndex(u => new { u.TenantId, u.EmailNormalized }).IsUnique();

        builder.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(80).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(80).IsRequired();
        builder.Property(u => u.Phone).HasMaxLength(20);
        builder.Property(u => u.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(u => u.FailedLoginAttempts).HasDefaultValue(0);
        builder.Property(u => u.IsDeleted).HasDefaultValue(false);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();

        builder.HasQueryFilter(u => !u.IsDeleted);
        builder.Ignore(u => u.DomainEvents);
    }
}
