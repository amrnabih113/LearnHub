using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Cart.Commands.ClearCart;

public sealed record ClearCartCommand(Guid StudentId) : IRequest<Result<CartDto>>;
