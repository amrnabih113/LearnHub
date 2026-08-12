namespace LearnHub.Application.Features.Admin.Dtos;

public sealed record UserAdminSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? ImageUrl,
    IReadOnlyList<string> Roles,
    bool IsEmailVerified,
    DateTimeOffset CreatedAtUtc);

public sealed record UserAdminDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? ImageUrl,
    IReadOnlyList<string> Roles,
    bool IsEmailVerified,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyList<UserEnrollmentDto> Enrollments,
    IReadOnlyList<UserOrderDto> Orders,
    IReadOnlyList<UserSubscriptionDto> Subscriptions,
    IReadOnlyList<UserReviewDto> Reviews);

public sealed record UserEnrollmentDto(
    Guid EnrollmentId,
    Guid CourseId,
    string CourseTitle,
    string Status,
    decimal ProgressPercentage,
    DateTimeOffset EnrolledAtUtc);

public sealed record UserOrderDto(
    Guid OrderId,
    decimal TotalAmount,
    string Currency,
    string Status,
    DateTimeOffset CreatedAtUtc);

public sealed record UserSubscriptionDto(
    Guid SubscriptionId,
    string Tier,
    string Status,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset ExpiresAtUtc);

public sealed record UserReviewDto(
    Guid ReviewId,
    Guid CourseId,
    int Rating,
    string Comment,
    string Status,
    DateTimeOffset CreatedAtUtc);
