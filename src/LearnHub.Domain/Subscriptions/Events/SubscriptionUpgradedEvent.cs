using LearnHub.Domain.Common;

namespace LearnHub.Domain.Subscriptions.Events;

public sealed class SubscriptionUpgradedEvent : DomainEvent
{
    public SubscriptionUpgradedEvent(Guid subscriptionId, Guid studentId, LearnHub.Domain.Subscriptions.SubscriptionTier oldTier, LearnHub.Domain.Subscriptions.SubscriptionTier newTier, DateTimeOffset upgradedAtUtc)
    {
        SubscriptionId = subscriptionId;
        StudentId = studentId;
        OldTier = oldTier;
        NewTier = newTier;
        UpgradedAtUtc = upgradedAtUtc;
    }

    public Guid SubscriptionId { get; }
    public Guid StudentId { get; }
    public SubscriptionTier OldTier { get; }
    public SubscriptionTier NewTier { get; }
    public DateTimeOffset UpgradedAtUtc { get; }
}
