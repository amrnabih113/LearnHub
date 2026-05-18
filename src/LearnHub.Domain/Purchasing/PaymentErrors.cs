using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Purchasing;

public static class PaymentErrors
{
    public static Error OrderIdRequired
    => Error.Validation(code: "DomainError.Payment.OrderIdRequired",
    description: "Order id is required");

    public static Error AmountRequired
    => Error.Validation(code: "DomainError.Payment.AmountRequired",
    description: "Payment amount is required");

    public static Error ProviderRequired
    => Error.Validation(code: "DomainError.Payment.ProviderRequired",
    description: "Payment provider is required");

    public static Error NotInitiated
    => Error.Conflict(code: "DomainError.Payment.NotInitiated",
    description: "Payment cannot be modified in its current state");

    public static Error AlreadySucceeded
    => Error.Conflict(code: "DomainError.Payment.AlreadySucceeded",
    description: "Payment is already succeeded");

    public static Error AlreadyFailed
    => Error.Conflict(code: "DomainError.Payment.AlreadyFailed",
    description: "Payment is already failed");

    public static Error AlreadyRefunded
    => Error.Conflict(code: "DomainError.Payment.AlreadyRefunded",
    description: "Payment is already refunded");

    public static Error InvalidTransactionId
    => Error.Validation(code: "DomainError.Payment.InvalidTransactionId",
    description: "Transaction id is required");
}
