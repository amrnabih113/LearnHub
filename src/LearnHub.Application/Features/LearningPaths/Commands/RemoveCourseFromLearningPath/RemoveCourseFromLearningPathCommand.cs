using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Commands.RemoveCourseFromLearningPath;

public sealed record RemoveCourseFromLearningPathCommand(
    Guid LearningPathId,
    Guid CourseId
) : IRequest<Result<Updated>>;
