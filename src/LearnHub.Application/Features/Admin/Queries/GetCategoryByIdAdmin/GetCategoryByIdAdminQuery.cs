using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetCategoryByIdAdmin;

public sealed record GetCategoryByIdAdminQuery(Guid Id) : IRequest<Result<CategoryAdminDto>>;
