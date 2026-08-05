using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Subscriptions;

namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record CourseDetailsDto(
    Guid Id,
    string Title,
    string Description,
    InstructorDto? Instructor,
    CategoryDto? Category,
    IReadOnlyCollection<TagDto> Tags,
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
    string? Country,
    IReadOnlyCollection<SectionDto> Sections);