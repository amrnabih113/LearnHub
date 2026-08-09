using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Subscriptions;
using MediatR;

namespace LearnHub.Application.Features.Subscriptions.Commands.CreateSubscriptionPlan;

public sealed record CreateSubscriptionPlanCommand(
    string Name,
    SubscriptionTier Tier,
    BillingCycle BillingCycle,
    decimal PriceAmount,
    string Currency) : IRequest<Result<Guid>>;
