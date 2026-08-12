using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Subscriptions.Commands.CancelSubscription;

public sealed record CancelSubscriptionCommand(Guid StudentId) : IRequest<Result<Updated>>;
