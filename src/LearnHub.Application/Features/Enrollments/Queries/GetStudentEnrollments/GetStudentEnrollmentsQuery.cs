using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Queries.GetStudentEnrollments;

public sealed record GetStudentEnrollmentsQuery(
    Guid StudentId,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<EnrollmentDto>>>;
