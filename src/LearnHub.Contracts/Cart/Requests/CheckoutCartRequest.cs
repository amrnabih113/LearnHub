namespace LearnHub.Contracts.Cart.Requests;

public sealed record CheckoutCartRequest(
    string SuccessUrl,
    string CancelUrl);
