using LearnHub.Domain.Subscriptions;

namespace LearnHub.Application.Features.Subscriptions.Dtos;

public sealed record SubscriptionPlanDto(
    Guid Id,
    string Name,
    SubscriptionTier Tier,
    BillingCycle BillingCycle,
    decimal PriceAmount,
    string Currency,
    bool IsActive);
