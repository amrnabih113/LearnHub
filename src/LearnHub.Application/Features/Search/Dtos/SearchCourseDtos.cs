using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Subscriptions;

namespace LearnHub.Application.Features.Search.Dtos;

public enum SearchCourseSortBy
{
    Relevance = 0,
    Newest = 1,
    Oldest = 2,
    PriceLowToHigh = 3,
    PriceHighToLow = 4,
    HighestRated = 5,
    MostPopular = 6
}

public sealed record CourseSearchDto(
    Guid CourseId,
    string Title,
    string? ThumbnailUrl,
    string Description,
    Guid CategoryId,
    string CategoryName,
    Guid? InstructorId,
    string InstructorName,
    CourseLevel Level,
    string LanguageCode,
    string LanguageName,
    decimal Price,
    string Currency,
    bool IsFree,
    bool IsIncludedInSubscription,
    SubscriptionTier RequiredSubscriptionTier,
    double AverageRating,
    int RatingCount,
    int EnrollmentCount,
    DateTimeOffset CreatedAtUtc,
    double RelevanceScore = 0,
    bool IsEnrolled = false,
    EnrollmentStatus? EnrollmentStatus = null,
    bool CanAccess = false);

public sealed record SearchAutoCompleteDto(
    IReadOnlyList<AutoCompleteSuggestionDto> CourseSuggestions,
    IReadOnlyList<AutoCompleteSuggestionDto> CategorySuggestions,
    IReadOnlyList<AutoCompleteSuggestionDto> InstructorSuggestions,
    IReadOnlyList<AutoCompleteSuggestionDto> TagSuggestions);

public sealed record AutoCompleteSuggestionDto(
    Guid Id,
    string Text,
    string Category, // "Course", "Category", "Instructor", "Tag"
    string? ExtraInfo = null);
