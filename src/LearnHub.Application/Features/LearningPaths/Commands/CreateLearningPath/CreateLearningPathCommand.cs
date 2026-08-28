using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using MediatR;

namespace LearnHub.Application.Features.LearningPaths.Commands.CreateLearningPath;

public sealed record CreateLearningPathCommand(
    string Title,
    string? Slug,
    string Description,
    string ShortDescription,
    string? ThumbnailUrl,
    CourseLevel Level,
    Guid? OwnerId = null
) : IRequest<Result<LearningPathDto>>;
