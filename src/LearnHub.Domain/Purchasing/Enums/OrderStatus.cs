namespace LearnHub.Domain.Purchasing.Enums;

public enum OrderStatus
{
    Draft,
    PendingPayment,
    Paid,
    Failed,
    Cancelled,
    Refunded
}
