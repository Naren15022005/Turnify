namespace Turnify.Modules.Booking.Domain.Enums;

public enum AppointmentStatus
{
    PendingPayment = 0,
    Confirmed = 1,
    InProgress = 2,
    Completed = 3,
    NoShow = 4,
    CancelledByCustomer = 5,
    CancelledByBusiness = 6
}
