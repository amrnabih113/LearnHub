using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Enrollments.Commands.UpdateEnrollmentProgress;

public sealed record UpdateEnrollmentProgressCommand(
    Guid EnrollmentId,
    Guid LessonId,
    int WatchDurationSeconds,
    int TotalLessons,
    int? LessonDurationSeconds = null) : IRequest<Result<Updated>>;
