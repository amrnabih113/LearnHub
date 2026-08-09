using LearnHub.Application.Features.Subscriptions.Dtos;
using LearnHub.Domain.Subscriptions;

namespace LearnHub.Application.Features.Subscriptions.Mappers;

public static class SubscriptionMappingExtensions
{
    public static SubscriptionDto ToDto(this Subscription subscription)
    {
        return new SubscriptionDto(
            subscription.Id,
            subscription.StudentId,
            subscription.Tier,
            subscription.SubscriptionPlanId,
            subscription.Plan?.Name,
            subscription.BillingCycle,
            subscription.Status,
            subscription.StartedAtUtc,
            subscription.ExpiresAtUtc,
            subscription.TrialEndsAtUtc,
            subscription.CancelledAtUtc,
            subscription.AutoRenewEnabled);
    }

    public static SubscriptionPlanDto ToDto(this SubscriptionPlan plan)
    {
        return new SubscriptionPlanDto(
            plan.Id,
            plan.Name,
            plan.Tier,
            plan.BillingCycle,
            plan.Price.Amount,
            plan.Price.Currency,
            plan.IsActive);
    }
}
