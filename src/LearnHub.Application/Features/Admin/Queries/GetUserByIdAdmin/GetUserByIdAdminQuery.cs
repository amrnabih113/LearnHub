using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetUserByIdAdmin;

public sealed record GetUserByIdAdminQuery(Guid Id) : IRequest<Result<UserAdminDetailDto>>;
