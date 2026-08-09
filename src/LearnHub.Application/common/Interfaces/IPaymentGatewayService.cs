using LearnHub.Domain.Common.Results;

namespace LearnHub.Application.common.Interfaces;

public enum PaymentType
{
    CoursePurchase,
    SubscriptionPurchase
}

public sealed record CreateCheckoutSessionArgs(
    Guid UserId,
    string UserEmail,
    PaymentType PaymentType,
    Guid TargetId,
    string ItemTitle,
    decimal Amount,
    string Currency,
    string SuccessUrl,
    string CancelUrl,
    Dictionary<string, string>? Metadata = null);

public sealed record CheckoutSessionResult(
    string SessionId,
    string CheckoutUrl,
    string? PaymentIntentId,
    string? CustomerId);

public interface IPaymentGatewayService
{
    string ProviderName { get; }

    Task<Result<CheckoutSessionResult>> CreateCheckoutSessionAsync(
        CreateCheckoutSessionArgs args,
        CancellationToken cancellationToken = default);
}
