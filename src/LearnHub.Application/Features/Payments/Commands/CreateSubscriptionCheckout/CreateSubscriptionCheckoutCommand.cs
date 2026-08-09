using LearnHub.Application.Features.Payments.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Payments.Commands.CreateSubscriptionCheckout;

public sealed record CreateSubscriptionCheckoutCommand(
    Guid StudentId,
    Guid SubscriptionPlanId,
    string SuccessUrl,
    string CancelUrl) : IRequest<Result<CheckoutSessionDto>>;
