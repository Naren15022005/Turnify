using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Catalog.Domain.Entities;

namespace Turnify.Modules.Catalog.Infrastructure.Persistence.Configurations;

public sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.PublicId).HasColumnType("char(26)").IsRequired();
        builder.HasIndex(s => s.PublicId).IsUnique();
        builder.HasIndex(s => s.TenantId);
        builder.Property(s => s.Name).HasMaxLength(150).IsRequired();
        builder.Property(s => s.Description).HasColumnType("text");
        builder.Property(s => s.Price).HasColumnType("decimal(10,2)");
        builder.Property(s => s.DepositAmount).HasColumnType("decimal(10,2)");
        builder.Property(s => s.ColorHex).HasColumnType("char(7)");
        builder.Property(s => s.IsActive).HasDefaultValue(true);
        builder.Property(s => s.IsDeleted).HasDefaultValue(false);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();

        builder.HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(s => !s.IsDeleted);
        builder.Ignore(s => s.DomainEvents);
    }
}
