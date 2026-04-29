using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Catalog.Domain.Entities;

namespace Turnify.Modules.Catalog.Infrastructure.Persistence.Configurations;

public sealed class StaffLocationConfiguration : IEntityTypeConfiguration<StaffLocation>
{
    public void Configure(EntityTypeBuilder<StaffLocation> builder)
    {
        builder.ToTable("staff_locations");
        builder.HasKey(sl => sl.Id);
        builder.Property(sl => sl.Id).ValueGeneratedOnAdd();

        builder.HasIndex(sl => new { sl.StaffId, sl.LocationId }).IsUnique();
        builder.HasIndex(sl => sl.TenantId);

        builder.HasOne(sl => sl.Staff)
            .WithMany(s => s.StaffLocations)
            .HasForeignKey(sl => sl.StaffId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sl => sl.Location)
            .WithMany()
            .HasForeignKey(sl => sl.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(sl => sl.CreatedAt).IsRequired();
        builder.Property(sl => sl.UpdatedAt).IsRequired();
    }
}
