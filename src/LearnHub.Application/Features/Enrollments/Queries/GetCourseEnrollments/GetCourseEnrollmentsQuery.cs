using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Queries.GetCourseEnrollments;

public sealed record GetCourseEnrollmentsQuery(
    Guid CourseId,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<EnrollmentDto>>>;
