using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Cart.Commands.AddToCart;

public sealed record AddToCartCommand(Guid StudentId, Guid CourseId) : IRequest<Result<CartDto>>;
