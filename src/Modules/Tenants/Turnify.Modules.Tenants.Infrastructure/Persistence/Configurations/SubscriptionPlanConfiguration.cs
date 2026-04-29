using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnify.Modules.Tenants.Domain.Entities;

namespace Turnify.Modules.Tenants.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("subscription_plans");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Code).HasMaxLength(30).IsRequired();
        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.Name).HasMaxLength(60).IsRequired();
        builder.Property(p => p.PriceMonthlyCop).HasColumnType("decimal(10,2)");
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();

        builder.HasData(
            new { Id = 1, Code = "starter", Name = "Starter", PriceMonthlyCop = 59000m, MaxLocations = 1, MaxAppointmentsMonth = (int?)200, HasWhatsApp = false, HasOnlinePayments = false, HasApi = false, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 2, Code = "pro", Name = "Pro", PriceMonthlyCop = 129000m, MaxLocations = 2, MaxAppointmentsMonth = (int?)1500, HasWhatsApp = true, HasOnlinePayments = true, HasApi = false, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new { Id = 3, Code = "business", Name = "Business", PriceMonthlyCop = 249000m, MaxLocations = 5, MaxAppointmentsMonth = (int?)null, HasWhatsApp = true, HasOnlinePayments = true, HasApi = true, IsDeleted = false, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
