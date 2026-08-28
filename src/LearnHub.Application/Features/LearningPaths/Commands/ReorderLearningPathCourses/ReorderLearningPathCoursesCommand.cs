using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Commands.ReorderLearningPathCourses;

public sealed record ReorderLearningPathCoursesCommand(
    Guid LearningPathId,
    List<Guid> OrderedCourseIds
) : IRequest<Result<Updated>>;
