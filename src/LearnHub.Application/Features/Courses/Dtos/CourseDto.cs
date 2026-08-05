using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Subscriptions;

namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record CourseDto(
    Guid Id,
    string Title,
    string Description,
    Guid InstructorId,
    Guid CategoryId,
    string? ThumbnailUrl,
    CourseLevel Level,
    CourseStatus Status,
    decimal PriceAmount,
    string Currency,
    bool IsIncludedInSubscription,
    SubscriptionTier RequiredSubscriptionTier,
    string LanguageCode,
    string LanguageName,
    string? Country);