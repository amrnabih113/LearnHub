using LearnHub.Application.Features.Instructor.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Instructor.Queries.GetInstructorAnalytics;

public sealed record GetInstructorAnalyticsQuery(Guid InstructorId, Guid? CourseId = null)
    : IRequest<Result<IReadOnlyList<InstructorCourseAnalyticsDto>>>;
