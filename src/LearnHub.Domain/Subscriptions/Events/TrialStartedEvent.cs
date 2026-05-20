using LearnHub.Domain.Common;

namespace LearnHub.Domain.Subscriptions.Events;

public sealed class TrialStartedEvent : DomainEvent
{
    public TrialStartedEvent(Guid subscriptionId, Guid studentId, LearnHub.Domain.Subscriptions.SubscriptionTier tier, DateTimeOffset startedAtUtc, DateTimeOffset trialEndsAtUtc)
    {
        SubscriptionId = subscriptionId;
        StudentId = studentId;
        Tier = tier;
        StartedAtUtc = startedAtUtc;
        TrialEndsAtUtc = trialEndsAtUtc;
    }

    public Guid SubscriptionId { get; }
    public Guid StudentId { get; }
    public SubscriptionTier Tier { get; }
    public DateTimeOffset StartedAtUtc { get; }
    public DateTimeOffset TrialEndsAtUtc { get; }
}
