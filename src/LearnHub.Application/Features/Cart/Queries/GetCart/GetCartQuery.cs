using LearnHub.Application.Features.Cart.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Cart.Queries.GetCart;

public sealed record GetCartQuery(Guid StudentId) : IRequest<Result<CartDto>>;
