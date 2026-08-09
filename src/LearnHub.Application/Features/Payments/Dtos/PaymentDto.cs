namespace LearnHub.Application.Features.Payments.Dtos;

public sealed record PaymentDto(
    Guid PaymentId,
    Guid UserId,
    string PaymentType,
    string? StripeSessionId,
    string? StripePaymentIntentId,
    string? StripeCustomerId,
    decimal Amount,
    string Currency,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PaidAt,
    string? FailureReason);
