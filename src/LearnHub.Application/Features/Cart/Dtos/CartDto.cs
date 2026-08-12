namespace LearnHub.Application.Features.Cart.Dtos;

public sealed record CartDto(
    Guid CartId,
    Guid StudentId,
    string Currency,
    IReadOnlyList<CartItemDto> Items,
    decimal OriginalSubtotal,
    decimal SubscriptionDiscount,
    decimal PayableSubtotal,
    string? CouponCode,
    decimal CouponDiscount,
    decimal TotalPayableAmount);
