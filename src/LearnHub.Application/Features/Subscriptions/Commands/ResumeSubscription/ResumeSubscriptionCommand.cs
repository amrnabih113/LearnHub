using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Subscriptions.Commands.ResumeSubscription;

public sealed record ResumeSubscriptionCommand(Guid StudentId) : IRequest<Result<Updated>>;
