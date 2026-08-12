using LearnHub.Application.Features.Subscriptions.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Subscriptions.Queries.GetSubscriptionHistory;

public sealed record GetSubscriptionHistoryQuery(Guid StudentId) : IRequest<Result<List<SubscriptionDto>>>;
