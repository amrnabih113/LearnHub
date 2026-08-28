using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Commands.UpdateLearningPath;

public sealed record UpdateLearningPathCommand(
    Guid LearningPathId,
    string Title,
    string? Slug,
    string Description,
    string ShortDescription,
    string? ThumbnailUrl,
    CourseLevel Level
) : IRequest<Result<LearningPathDto>>;
