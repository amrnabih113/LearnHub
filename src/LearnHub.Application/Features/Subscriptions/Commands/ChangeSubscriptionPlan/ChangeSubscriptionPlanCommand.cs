using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Subscriptions.Commands.ChangeSubscriptionPlan;

public sealed record ChangeSubscriptionPlanCommand(
    Guid StudentId,
    Guid NewPlanId) : IRequest<Result<Updated>>;
