using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.LearningPaths.Enums;

namespace LearnHub.Application.Features.LearningPaths.Dtos;

public sealed record LearningPathDto(
    Guid Id,
    string Title,
    string Slug,
    string Description,
    string ShortDescription,
    string? ThumbnailUrl,
    CourseLevel Level,
    LearningPathStatus Status,
    Guid? OwnerId,
    string? OwnerName,
    int TotalCourses,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? PublishedAtUtc);

public sealed record LearningPathDetailDto(
    Guid Id,
    string Title,
    string Slug,
    string Description,
    string ShortDescription,
    string? ThumbnailUrl,
    CourseLevel Level,
    LearningPathStatus Status,
    Guid? OwnerId,
    string? OwnerName,
    int TotalCourses,
    IReadOnlyList<LearningPathCourseDto> Courses,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? PublishedAtUtc);

public sealed record LearningPathCourseDto(
    Guid CourseId,
    string Title,
    string? ThumbnailUrl,
    string CategoryName,
    string InstructorName,
    CourseLevel Level,
    decimal Price,
    string Currency,
    bool IsFree,
    bool IsIncludedInSubscription,
    int Order,
    bool IsRequired,
    double AverageRating,
    int EnrollmentCount);

public sealed record LearningPathProgressDto(
    Guid LearningPathId,
    string PathTitle,
    int TotalCourses,
    int CompletedCourses,
    decimal ProgressPercentage,
    Guid? CurrentCourseId,
    string? CurrentCourseTitle,
    Guid? NextCourseId,
    string? NextCourseTitle,
    bool IsCompleted);
