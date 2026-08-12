using LearnHub.Application.Features.Subscriptions.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Subscriptions.Queries.GetCurrentSubscription;

public sealed record GetCurrentSubscriptionQuery(Guid StudentId) : IRequest<Result<SubscriptionDto>>;
