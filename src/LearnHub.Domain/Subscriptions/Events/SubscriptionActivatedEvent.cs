using LearnHub.Domain.Common;

namespace LearnHub.Domain.Subscriptions.Events;

public sealed class SubscriptionActivatedEvent : DomainEvent
{
    public SubscriptionActivatedEvent(Guid subscriptionId, string studentId, LearnHub.Domain.Subscriptions.SubscriptionTier tier, DateTimeOffset startedAtUtc, DateTimeOffset expiresAtUtc)
    {
        SubscriptionId = subscriptionId;
        StudentId = studentId;
        Tier = tier;
        StartedAtUtc = startedAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid SubscriptionId { get; }
    public string StudentId { get; }
    public SubscriptionTier Tier { get; }
    public DateTimeOffset StartedAtUtc { get; }
    public DateTimeOffset ExpiresAtUtc { get; }
}
