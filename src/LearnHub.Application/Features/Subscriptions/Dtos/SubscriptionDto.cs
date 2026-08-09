using LearnHub.Domain.Subscriptions;

namespace LearnHub.Application.Features.Subscriptions.Dtos;

public sealed record SubscriptionDto(
    Guid Id,
    Guid StudentId,
    SubscriptionTier Tier,
    Guid SubscriptionPlanId,
    string? PlanName,
    BillingCycle BillingCycle,
    SubscriptionStatus Status,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset? TrialEndsAtUtc,
    DateTimeOffset? CancelledAtUtc,
    bool AutoRenewEnabled);
