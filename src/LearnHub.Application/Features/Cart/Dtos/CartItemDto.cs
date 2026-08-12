namespace LearnHub.Application.Features.Cart.Dtos;

public sealed record CartItemDto(
    Guid CourseId,
    string CourseTitle,
    decimal OriginalUnitPrice,
    bool IsFree,
    bool IsCoveredBySubscription,
    decimal PayableUnitPrice,
    string Currency);
