using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Cart.Commands.CheckoutCart;

public sealed record CheckoutCartCommand(
    Guid StudentId,
    string SuccessUrl,
    string CancelUrl) : IRequest<Result<CartCheckoutDto>>;
