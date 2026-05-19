using LearnHub.Domain.Common;

namespace LearnHub.Domain.Subscriptions.Events;

public sealed class SubscriptionPaymentRefundedEvent : DomainEvent
{
    public SubscriptionPaymentRefundedEvent(Guid paymentId, Guid subscriptionId, string? refundReason, DateTimeOffset refundedAtUtc)
    {
        PaymentId = paymentId;
        SubscriptionId = subscriptionId;
        RefundReason = refundReason;
        RefundedAtUtc = refundedAtUtc;
    }

    public Guid PaymentId { get; }
    public Guid SubscriptionId { get; }
    public string? RefundReason { get; }
    public DateTimeOffset RefundedAtUtc { get; }
}
