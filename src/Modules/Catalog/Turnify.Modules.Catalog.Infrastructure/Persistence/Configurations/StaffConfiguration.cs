using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Catalog.Domain.Entities;

namespace Turnify.Modules.Catalog.Infrastructure.Persistence.Configurations;

public sealed class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("staff");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.PublicId).HasColumnType("char(26)").IsRequired();
        builder.HasIndex(s => s.PublicId).IsUnique();
        builder.HasIndex(s => s.TenantId);
        builder.Property(s => s.FirstName).HasMaxLength(80).IsRequired();
        builder.Property(s => s.LastName).HasMaxLength(80).IsRequired();
        builder.Property(s => s.Email).HasMaxLength(150);
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.ProfessionalTitle).HasMaxLength(100);
        builder.Property(s => s.Bio).HasColumnType("text");
        builder.Property(s => s.PhotoUrl).HasMaxLength(500);
        builder.Property(s => s.IsBookable).HasDefaultValue(true);
        builder.Property(s => s.IsActive).HasDefaultValue(true);
        builder.Property(s => s.IsDeleted).HasDefaultValue(false);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();
        builder.HasQueryFilter(s => !s.IsDeleted);
        builder.Ignore(s => s.DomainEvents);
    }
}
