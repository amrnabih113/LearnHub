using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Cart.Commands.RemoveCouponFromCart;

public sealed record RemoveCouponFromCartCommand(Guid StudentId) : IRequest<Result<CartDto>>;
