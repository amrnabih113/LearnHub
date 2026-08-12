namespace LearnHub.Application.Features.Admin.Dtos;

public sealed record OrderAdminSummaryDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    string StudentEmail,
    decimal TotalAmount,
    string Currency,
    string Status,
    int ItemsCount,
    DateTimeOffset CreatedAtUtc);

public sealed record OrderAdminDetailDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    string StudentEmail,
    decimal SubtotalAmount,
    decimal DiscountAmount,
    decimal TotalAmount,
    string Currency,
    string Status,
    string? CouponCode,
    IReadOnlyList<OrderItemAdminDto> Items,
    IReadOnlyList<PaymentAdminSummaryDto> Payments,
    DateTimeOffset CreatedAtUtc);

public sealed record OrderItemAdminDto(
    Guid CourseId,
    string CourseTitle,
    decimal UnitPrice);

public sealed record PaymentAdminSummaryDto(
    Guid Id,
    Guid OrderId,
    string Provider,
    string Status,
    decimal Amount,
    string Currency,
    string? TransactionId,
    string? ProviderReference,
    string? FailureReason,
    DateTimeOffset CreatedAtUtc);

public sealed record SubscriptionAdminSummaryDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    string StudentEmail,
    string Tier,
    string Status,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset CreatedAtUtc);

public sealed record SubscriptionAdminDetailDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    string StudentEmail,
    string Tier,
    string Status,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset? CanceledAtUtc,
    IReadOnlyList<SubscriptionPaymentAdminDto> PaymentHistory,
    DateTimeOffset CreatedAtUtc);

public sealed record SubscriptionPaymentAdminDto(
    Guid Id,
    decimal Amount,
    string Currency,
    string Status,
    string? GatewayTransactionId,
    string? FailureReason,
    DateTimeOffset CreatedAtUtc);
