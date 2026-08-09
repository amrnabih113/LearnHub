using LearnHub.Application.Features.Subscriptions.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Subscriptions.Queries.GetSubscriptionPlans;

public sealed record GetSubscriptionPlansQuery(bool OnlyActive = true) : IRequest<Result<List<SubscriptionPlanDto>>>;
