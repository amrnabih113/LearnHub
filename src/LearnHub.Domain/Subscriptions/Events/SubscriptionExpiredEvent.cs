using LearnHub.Domain.Common;

namespace LearnHub.Domain.Subscriptions.Events;

public sealed class SubscriptionExpiredEvent : DomainEvent
{
    public SubscriptionExpiredEvent(Guid subscriptionId, Guid studentId, DateTimeOffset expiredAtUtc)
    {
        SubscriptionId = subscriptionId;
        StudentId = studentId;
        ExpiredAtUtc = expiredAtUtc;
    }

    public Guid SubscriptionId { get; }
    public Guid StudentId { get; }
    public DateTimeOffset ExpiredAtUtc { get; }
}
