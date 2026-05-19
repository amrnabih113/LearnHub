using LearnHub.Domain.Common;

namespace LearnHub.Domain.Subscriptions.Events;

public sealed class SubscriptionPaymentCreatedEvent : DomainEvent
{
    public SubscriptionPaymentCreatedEvent(Guid paymentId, Guid subscriptionId, decimal amount, string currency, DateTimeOffset dueAtUtc)
    {
        PaymentId = paymentId;
        SubscriptionId = subscriptionId;
        Amount = amount;
        Currency = currency;
        DueAtUtc = dueAtUtc;
    }

    public Guid PaymentId { get; }
    public Guid SubscriptionId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public DateTimeOffset DueAtUtc { get; }
}
