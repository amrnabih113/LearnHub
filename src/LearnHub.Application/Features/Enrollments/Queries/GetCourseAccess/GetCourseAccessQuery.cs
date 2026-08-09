using LearnHub.Application.Features.Enrollments.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Queries.GetCourseAccess;

public sealed record GetCourseAccessQuery(Guid CourseId, Guid StudentId) : IRequest<Result<CourseAccessResult>>;
