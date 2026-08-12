using LearnHub.Application.Features.Subscriptions.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Subscriptions.Queries.GetSubscriptionPlanById;

public sealed record GetSubscriptionPlanByIdQuery(Guid Id) : IRequest<Result<SubscriptionPlanDto>>;
