using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Purchasing.Events;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Purchasing.Payments;

public sealed class Payment : AuditableEntity
{
    public Guid OrderId { get; private set; }
    public PaymentProvider Provider { get; private set; }
    public Money Amount { get; private set; } = default!;
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public string? ProviderReference { get; private set; }
    public string? FailureReason { get; private set; }
    public string? RefundReason { get; private set; }
    public DateTimeOffset? SucceededAtUtc { get; private set; }
    public DateTimeOffset? FailedAtUtc { get; private set; }
    public DateTimeOffset? RefundedAtUtc { get; private set; }

    private Payment() { }

    private Payment(Guid id, Guid orderId, PaymentProvider provider, Money amount) : base(id)
    {
        OrderId = orderId;
        Provider = provider;
        Amount = amount;
        Status = PaymentStatus.Initiated;
    }

    public static Result<Payment> Create(Guid id,
                                         Guid orderId,
                                         PaymentProvider provider,
                                         Money amount)
    {
        if (orderId == Guid.Empty)
        {
            return PaymentErrors.OrderIdRequired;
        }

        if (amount.Amount < 0)
        {
            return PaymentErrors.AmountRequired;
        }

        if (!Enum.IsDefined(typeof(PaymentProvider), provider))
        {
            return PaymentErrors.ProviderRequired;
        }

        return new Payment(id, orderId, provider, amount);
    }

    public Result<Updated> MarkSucceeded(string transactionId, string providerReference, DateTimeOffset succeededAtUtc)
    {
        if (Status == PaymentStatus.Succeeded)
        {
            return PaymentErrors.AlreadySucceeded;
        }

        if (Status is PaymentStatus.Failed or PaymentStatus.Refunded)
        {
            return PaymentErrors.NotInitiated;
        }

        TransactionId = transactionId;
        ProviderReference = providerReference;
        Status = PaymentStatus.Succeeded;
        SucceededAtUtc = succeededAtUtc;
        UpdatedAtUtc = succeededAtUtc;
        AddDomainEvent(new PaymentSucceededDomainEvent(Id, OrderId, transactionId));
        return Result.Updated;
    }

    public Result<Updated> MarkFailed(string reason, DateTimeOffset failedAtUtc)
    {
        if (Status == PaymentStatus.Succeeded)
        {
            return PaymentErrors.AlreadySucceeded;
        }

        if (Status is PaymentStatus.Failed or PaymentStatus.Refunded)
        {
            return PaymentErrors.AlreadyFailed;
        }

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        FailedAtUtc = failedAtUtc;
        UpdatedAtUtc = failedAtUtc;
        AddDomainEvent(new PaymentFailedDomainEvent(Id, OrderId, reason));
        return Result.Updated;
    }

    public Result<Updated> Refund(string reason, DateTimeOffset refundedAtUtc)
    {
        if (Status != PaymentStatus.Succeeded)
        {
            return PaymentErrors.NotInitiated;
        }

        Status = PaymentStatus.Refunded;
        RefundReason = reason;
        RefundedAtUtc = refundedAtUtc;
        UpdatedAtUtc = refundedAtUtc;
        AddDomainEvent(new RefundIssuedDomainEvent(Id, OrderId, reason));
        return Result.Updated;
    }
}
