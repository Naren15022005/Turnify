using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Scheduling.Domain.Entities;

namespace Turnify.Modules.Scheduling.Infrastructure.Persistence.Configurations;

public sealed class StaffTimeOffConfiguration : IEntityTypeConfiguration<StaffTimeOff>
{
    public void Configure(EntityTypeBuilder<StaffTimeOff> builder)
    {
        builder.ToTable("staff_time_off");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.PublicId).HasColumnType("char(26)").IsRequired();
        builder.HasIndex(t => t.PublicId).IsUnique();
        builder.HasIndex(t => new { t.StaffId, t.StartsAt });
        builder.Property(t => t.TenantId).IsRequired();
        builder.Property(t => t.StaffId).IsRequired();
        builder.Property(t => t.StartsAt).IsRequired();
        builder.Property(t => t.EndsAt).IsRequired();
        builder.Property(t => t.Reason).HasMaxLength(500);
        builder.Property(t => t.IsDeleted).HasDefaultValue(false);
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt).IsRequired();
        builder.Ignore(t => t.DomainEvents);
    }
}
