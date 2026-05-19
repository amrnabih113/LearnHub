using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Enums;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Subscriptions;

public sealed class SubscriptionPayment : AuditableEntity
{
    public Guid SubscriptionId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public PaymentStatus Status { get; private set; }
    public int AttemptCount { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? FailureReason { get; private set; }
    public string? RefundReason { get; private set; }
    public DateTimeOffset DueAtUtc { get; private set; }
    public DateTimeOffset? SucceededAtUtc { get; private set; }
    public DateTimeOffset? FailedAtUtc { get; private set; }
    public DateTimeOffset? RefundedAtUtc { get; private set; }

    // EF navigation
    public Subscription? Subscription { get; private set; }

    private SubscriptionPayment() { }

    private SubscriptionPayment(Guid id, Guid subscriptionId, Money amount, DateTimeOffset dueAtUtc) : base(id)
    {
        SubscriptionId = subscriptionId;
        Amount = amount;
        Status = PaymentStatus.Initiated;
        AttemptCount = 0;
        DueAtUtc = dueAtUtc;
    }

    public static Result<SubscriptionPayment> Create(Guid id, Guid subscriptionId, Money amount, DateTimeOffset dueAtUtc)
    {
        if (subscriptionId == Guid.Empty)
        {
            return Error.Validation(code: "DomainError.SubscriptionPayment.SubscriptionIdRequired", description: "Subscription id is required");
        }

        if (dueAtUtc <= DateTimeOffset.UtcNow.AddMinutes(-1))
        {
            return Error.Validation(code: "DomainError.SubscriptionPayment.InvalidDueDate", description: "Due date must be in the future");
        }

        return new SubscriptionPayment(id, subscriptionId, amount, dueAtUtc);
    }

    public Result<Updated> MarkProcessing()
    {
        Status = PaymentStatus.Processing;
        AttemptCount++;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new Events.SubscriptionPaymentCreatedEvent(Id, SubscriptionId, Amount.Amount, Amount.Currency, DueAtUtc));
        return Result.Updated;
    }

    public Result<Updated> MarkSucceeded(string gatewayTransactionId, DateTimeOffset succeededAtUtc)
    {
        GatewayTransactionId = gatewayTransactionId;
        Status = PaymentStatus.Succeeded;
        SucceededAtUtc = succeededAtUtc;
        UpdatedAtUtc = succeededAtUtc;
        AddDomainEvent(new Events.SubscriptionPaymentSucceededEvent(Id, SubscriptionId, gatewayTransactionId, Amount.Amount, Amount.Currency, succeededAtUtc));
        return Result.Updated;
    }

    public Result<Updated> MarkFailed(DateTimeOffset failedAtUtc, string? reason = null)
    {
        Status = PaymentStatus.Failed;
        FailedAtUtc = failedAtUtc;
        FailureReason = reason;
        UpdatedAtUtc = failedAtUtc;
        AddDomainEvent(new Events.SubscriptionPaymentFailedEvent(Id, SubscriptionId, AttemptCount, reason, failedAtUtc));
        return Result.Updated;
    }

    public Result<Updated> MarkRefunded(string? refundReason, DateTimeOffset refundedAtUtc)
    {
        Status = PaymentStatus.Refunded;
        RefundReason = refundReason;
        RefundedAtUtc = refundedAtUtc;
        UpdatedAtUtc = refundedAtUtc;
        AddDomainEvent(new Events.SubscriptionPaymentRefundedEvent(Id, SubscriptionId, refundReason, refundedAtUtc));
        return Result.Updated;
    }
}
