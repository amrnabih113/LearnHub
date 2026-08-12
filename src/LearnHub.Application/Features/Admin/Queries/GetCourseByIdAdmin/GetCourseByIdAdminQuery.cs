using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetCourseByIdAdmin;

public sealed record GetCourseByIdAdminQuery(Guid Id) : IRequest<Result<CourseAdminDetailDto>>;
