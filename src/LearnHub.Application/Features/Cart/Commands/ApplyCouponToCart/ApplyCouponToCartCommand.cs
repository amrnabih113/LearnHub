using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Cart.Commands.ApplyCouponToCart;

public sealed record ApplyCouponToCartCommand(Guid StudentId, string CouponCode) : IRequest<Result<CartDto>>;
