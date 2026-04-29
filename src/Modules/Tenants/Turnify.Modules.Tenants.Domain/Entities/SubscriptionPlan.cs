using Turnify.Shared.Kernel.Domain;

namespace Turnify.Modules.Tenants.Domain.Entities;

public sealed class SubscriptionPlan : Entity<int>
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public decimal PriceMonthlyCop { get; private set; }
    public int? MaxStaff { get; private set; }
    public int MaxLocations { get; private set; }
    public int? MaxAppointmentsMonth { get; private set; }
    public bool HasWhatsApp { get; private set; }
    public bool HasOnlinePayments { get; private set; }
    public bool HasApi { get; private set; }

    private SubscriptionPlan() { }
}
