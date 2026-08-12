using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Admin.Queries.GetCoursesAdmin;

public sealed record GetCoursesAdminQuery(
    string? Search = null,
    string? Status = null,
    Guid? InstructorId = null,
    Guid? CategoryId = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<CourseAdminSummaryDto>>>;
