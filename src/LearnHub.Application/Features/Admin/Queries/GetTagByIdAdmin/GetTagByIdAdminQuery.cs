using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetTagByIdAdmin;

public sealed record GetTagByIdAdminQuery(Guid Id) : IRequest<Result<TagAdminDto>>;
