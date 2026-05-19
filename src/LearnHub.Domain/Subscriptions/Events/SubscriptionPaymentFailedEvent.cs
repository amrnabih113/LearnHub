using LearnHub.Domain.Common;

namespace LearnHub.Domain.Subscriptions.Events;

public sealed class SubscriptionPaymentFailedEvent : DomainEvent
{
    public SubscriptionPaymentFailedEvent(Guid paymentId, Guid subscriptionId, int attemptCount, string? reason, DateTimeOffset failedAtUtc)
    {
        PaymentId = paymentId;
        SubscriptionId = subscriptionId;
        AttemptCount = attemptCount;
        Reason = reason;
        FailedAtUtc = failedAtUtc;
    }

    public Guid PaymentId { get; }
    public Guid SubscriptionId { get; }
    public int AttemptCount { get; }
    public string? Reason { get; }
    public DateTimeOffset FailedAtUtc { get; }
}
