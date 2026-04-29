using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Catalog.Domain.Entities;

namespace Turnify.Modules.Catalog.Infrastructure.Persistence.Configurations;

public sealed class StaffServiceConfiguration : IEntityTypeConfiguration<StaffService>
{
    public void Configure(EntityTypeBuilder<StaffService> builder)
    {
        builder.ToTable("staff_services");
        builder.HasKey(ss => ss.Id);
        builder.Property(ss => ss.Id).ValueGeneratedOnAdd();

        builder.HasIndex(ss => new { ss.StaffId, ss.ServiceId }).IsUnique();
        builder.HasIndex(ss => ss.TenantId);

        builder.HasOne(ss => ss.Staff)
            .WithMany(s => s.StaffServices)
            .HasForeignKey(ss => ss.StaffId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ss => ss.Service)
            .WithMany(s => s.StaffServices)
            .HasForeignKey(ss => ss.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ss => ss.CreatedAt).IsRequired();
        builder.Property(ss => ss.UpdatedAt).IsRequired();
    }
}
