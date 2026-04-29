using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Catalog.Domain.Entities;

namespace Turnify.Modules.Catalog.Infrastructure.Persistence.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();
        builder.Property(l => l.PublicId).HasColumnType("char(26)").IsRequired();
        builder.HasIndex(l => l.PublicId).IsUnique();
        builder.HasIndex(l => l.TenantId);
        builder.Property(l => l.Name).HasMaxLength(150).IsRequired();
        builder.Property(l => l.Address).HasMaxLength(300);
        builder.Property(l => l.City).HasMaxLength(100);
        builder.Property(l => l.Department).HasMaxLength(100);
        builder.Property(l => l.Phone).HasMaxLength(20);
        builder.Property(l => l.Timezone).HasMaxLength(50).HasDefaultValue("America/Bogota");
        builder.Property(l => l.Latitude).HasColumnType("decimal(10,7)");
        builder.Property(l => l.Longitude).HasColumnType("decimal(10,7)");
        builder.Property(l => l.IsActive).HasDefaultValue(true);
        builder.Property(l => l.IsDeleted).HasDefaultValue(false);
        builder.Property(l => l.CreatedAt).IsRequired();
        builder.Property(l => l.UpdatedAt).IsRequired();
        builder.HasQueryFilter(l => !l.IsDeleted);
        builder.Ignore(l => l.DomainEvents);
    }
}
