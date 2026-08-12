using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Cart.Commands.RemoveFromCart;

public sealed record RemoveFromCartCommand(Guid StudentId, Guid CourseId) : IRequest<Result<CartDto>>;
