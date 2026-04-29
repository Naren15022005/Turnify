using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Tenants.Domain.Entities;

namespace Turnify.Modules.Tenants.Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.PublicId).HasColumnType("char(26)").IsRequired();
        builder.HasIndex(t => t.PublicId).IsUnique();

        builder.Property(t => t.Slug).HasMaxLength(60).IsRequired();
        builder.HasIndex(t => t.Slug).IsUnique();

        builder.Property(t => t.Name).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Nit).HasMaxLength(20);
        builder.Property(t => t.Timezone).HasMaxLength(50).HasDefaultValue("America/Bogota");
        builder.Property(t => t.Currency).HasColumnType("char(3)").HasDefaultValue("COP");
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt).IsRequired();
        builder.Property(t => t.IsDeleted).HasDefaultValue(false);

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.Ignore(t => t.DomainEvents);
    }
}
