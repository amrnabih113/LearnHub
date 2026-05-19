using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Subscriptions;

public static class SubscriptionErrors
{
    public static Error StudentIdRequired
    => Error.Validation(code: "DomainError.Subscription.StudentIdRequired",
    description: "Student id is required");

    public static Error NameRequired
    => Error.Validation(code: "DomainError.Subscription.NameRequired",
    description: "Subscription plan name is required");

    public static Error ExpirationRequired
    => Error.Validation(code: "DomainError.Subscription.ExpirationRequired",
    description: "Subscription expiration is required");

    public static Error InvalidTier
    => Error.Validation(code: "DomainError.Subscription.InvalidTier",
    description: "Subscription tier is invalid");

    public static Error InvalidBillingCycle
    => Error.Validation(code: "DomainError.Subscription.InvalidBillingCycle",
    description: "Billing cycle is invalid");

    public static Error TrialAlreadyUsed
    => Error.Conflict(code: "DomainError.Subscription.TrialAlreadyUsed",
    description: "Trial offer has already been used");

    public static Error TrialExpired
    => Error.Conflict(code: "DomainError.Subscription.TrialExpired",
    description: "Trial offer has expired");
}