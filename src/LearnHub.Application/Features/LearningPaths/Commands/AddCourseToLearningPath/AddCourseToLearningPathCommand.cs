using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Commands.AddCourseToLearningPath;

public sealed record AddCourseToLearningPathCommand(
    Guid LearningPathId,
    Guid CourseId,
    int? TargetOrder = null,
    bool IsRequired = true
) : IRequest<Result<Updated>>;
