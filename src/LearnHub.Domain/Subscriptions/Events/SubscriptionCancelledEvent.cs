using LearnHub.Domain.Common;

namespace LearnHub.Domain.Subscriptions.Events;

public sealed class SubscriptionCancelledEvent : DomainEvent
{
    public SubscriptionCancelledEvent(Guid subscriptionId, Guid studentId, DateTimeOffset cancelledAtUtc)
    {
        SubscriptionId = subscriptionId;
        StudentId = studentId;
        CancelledAtUtc = cancelledAtUtc;
    }

    public Guid SubscriptionId { get; }
    public Guid StudentId { get; }
    public DateTimeOffset CancelledAtUtc { get; }
}
