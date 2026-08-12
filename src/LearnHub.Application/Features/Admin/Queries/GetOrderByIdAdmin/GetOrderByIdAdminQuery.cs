using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetOrderByIdAdmin;

public sealed record GetOrderByIdAdminQuery(Guid Id) : IRequest<Result<OrderAdminDetailDto>>;
