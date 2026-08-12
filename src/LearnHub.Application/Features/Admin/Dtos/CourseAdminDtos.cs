namespace LearnHub.Application.Features.Admin.Dtos;

public sealed record CourseAdminSummaryDto(
    Guid Id,
    string Title,
    string Status,
    decimal PriceAmount,
    string Currency,
    Guid? InstructorId,
    string InstructorName,
    Guid CategoryId,
    string CategoryName,
    bool IsIncludedInSubscription,
    DateTimeOffset CreatedAtUtc);

public sealed record CourseAdminDetailDto(
    Guid Id,
    string Title,
    string Description,
    string Status,
    decimal PriceAmount,
    string Currency,
    Guid? InstructorId,
    string InstructorName,
    string InstructorEmail,
    Guid CategoryId,
    string CategoryName,
    string Level,
    bool IsIncludedInSubscription,
    string RequiredSubscriptionTier,
    int SectionsCount,
    int LessonsCount,
    int EnrollmentsCount,
    double AverageRating,
    int TotalReviews,
    DateTimeOffset CreatedAtUtc);
