using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Subscriptions;
using Microsoft.AspNetCore.Http;

namespace LearnHub.Contracts.Courses.Requests;

public sealed record CreateCourseRequest(
    string Title,
    string Description,
    Guid InstructorId,
    Guid CategoryId,
    CourseLevel Level,
    CourseStatus Status,
    decimal PriceAmount,
    string Currency,
    bool IsIncludedInSubscription,
    SubscriptionTier RequiredSubscriptionTier,
    string LanguageCode,
    string LanguageName,
    string? Country
    );
