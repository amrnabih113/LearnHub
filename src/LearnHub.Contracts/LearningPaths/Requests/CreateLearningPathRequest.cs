using LearnHub.Domain.Courses.Enums;

namespace LearnHub.Contracts.LearningPaths.Requests;

public sealed record CreateLearningPathRequest(
    string Title,
    string? Slug,
    string Description,
    string ShortDescription,
    string? ThumbnailUrl,
    CourseLevel Level);
