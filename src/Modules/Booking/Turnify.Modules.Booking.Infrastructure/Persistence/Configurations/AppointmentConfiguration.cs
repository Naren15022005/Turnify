using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Booking.Domain.Entities;

namespace Turnify.Modules.Booking.Infrastructure.Persistence.Configurations;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.PublicId).HasColumnType("char(26)").IsRequired();
        builder.HasIndex(a => a.PublicId).IsUnique();
        builder.HasIndex(a => new { a.StaffId, a.StartsAt });
        builder.HasIndex(a => new { a.TenantId, a.StartsAt });

        builder.Property(a => a.TenantId).IsRequired();
        builder.Property(a => a.StaffId).IsRequired();
        builder.Property(a => a.ServiceId).IsRequired();
        builder.Property(a => a.CustomerName).HasMaxLength(160).IsRequired();
        builder.Property(a => a.CustomerEmail).HasMaxLength(150);
        builder.Property(a => a.CustomerPhone).HasMaxLength(20);
        builder.Property(a => a.StartsAt).IsRequired();
        builder.Property(a => a.EndsAt).IsRequired();
        builder.Property(a => a.DurationMinutes).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.Notes).HasMaxLength(1000);
        builder.Property(a => a.CancellationReason).HasMaxLength(500);
        builder.Property(a => a.IsDeleted).HasDefaultValue(false);
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired();
        builder.Ignore(a => a.DomainEvents);
    }
}
