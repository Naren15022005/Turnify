using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Scheduling.Domain.Entities;

namespace Turnify.Modules.Scheduling.Infrastructure.Persistence.Configurations;

public sealed class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.ToTable("holidays");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedOnAdd();
        builder.Property(h => h.PublicId).HasColumnType("char(26)").IsRequired();
        builder.HasIndex(h => h.PublicId).IsUnique();
        builder.HasIndex(h => new { h.TenantId, h.Date }).IsUnique();
        builder.Property(h => h.TenantId).IsRequired();
        builder.Property(h => h.Date).IsRequired();
        builder.Property(h => h.Name).HasMaxLength(150).IsRequired();
        builder.Property(h => h.IsRecurring).HasDefaultValue(false);
        builder.Property(h => h.IsDeleted).HasDefaultValue(false);
        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
        builder.Ignore(h => h.DomainEvents);
    }
}
