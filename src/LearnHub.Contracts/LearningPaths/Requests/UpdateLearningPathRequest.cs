using LearnHub.Domain.Courses.Enums;

namespace LearnHub.Contracts.LearningPaths.Requests;

public sealed record UpdateLearningPathRequest(
    string Title,
    string? Slug,
    string Description,
    string ShortDescription,
    string? ThumbnailUrl,
    CourseLevel Level);
