namespace LearnHub.Contracts.Cart.Responses;

public sealed record CartCheckoutResponse(
    Guid OrderId,
    Guid? PaymentId,
    string? SessionId,
    string? CheckoutUrl,
    decimal Amount,
    string Currency,
    bool RequiresPayment);
