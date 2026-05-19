using LearnHub.Domain.Common;

namespace LearnHub.Domain.Subscriptions.Events;

public sealed class SubscriptionPaymentSucceededEvent : DomainEvent
{
    public SubscriptionPaymentSucceededEvent(Guid paymentId, Guid subscriptionId, string gatewayTransactionId, decimal amount, string currency, DateTimeOffset succeededAtUtc)
    {
        PaymentId = paymentId;
        SubscriptionId = subscriptionId;
        GatewayTransactionId = gatewayTransactionId;
        Amount = amount;
        Currency = currency;
        SucceededAtUtc = succeededAtUtc;
    }

    public Guid PaymentId { get; }
    public Guid SubscriptionId { get; }
    public string GatewayTransactionId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public DateTimeOffset SucceededAtUtc { get; }
}
