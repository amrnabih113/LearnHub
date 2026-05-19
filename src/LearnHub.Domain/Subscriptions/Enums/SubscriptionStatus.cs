namespace LearnHub.Domain.Subscriptions;

public enum SubscriptionStatus
{
    PendingActivation,
    Trialing,
    Active,
    Cancelled,
    Expired
}