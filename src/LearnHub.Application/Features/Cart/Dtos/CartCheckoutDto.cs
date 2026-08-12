namespace LearnHub.Application.Features.Cart.Dtos;

public sealed record CartCheckoutDto(
    Guid OrderId,
    Guid? PaymentId,
    string? SessionId,
    string? CheckoutUrl,
    decimal Amount,
    string Currency,
    bool RequiresPayment);
