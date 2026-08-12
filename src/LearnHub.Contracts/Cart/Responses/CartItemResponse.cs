namespace LearnHub.Contracts.Cart.Responses;

public sealed record CartItemResponse(
    Guid CourseId,
    string CourseTitle,
    decimal OriginalUnitPrice,
    bool IsFree,
    bool IsCoveredBySubscription,
    decimal PayableUnitPrice,
    string Currency);
