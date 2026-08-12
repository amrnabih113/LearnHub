namespace LearnHub.Contracts.Cart.Responses;

public sealed record CartResponse(
    Guid CartId,
    Guid StudentId,
    string Currency,
    IReadOnlyList<CartItemResponse> Items,
    decimal OriginalSubtotal,
    decimal SubscriptionDiscount,
    decimal PayableSubtotal,
    string? CouponCode,
    decimal CouponDiscount,
    decimal TotalPayableAmount);
