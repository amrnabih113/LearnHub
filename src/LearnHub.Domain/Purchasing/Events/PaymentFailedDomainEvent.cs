using LearnHub.Domain.Common;

namespace LearnHub.Domain.Purchasing.Events;

public sealed class PaymentFailedDomainEvent : DomainEvent
{
    public PaymentFailedDomainEvent(Guid paymentId, Guid orderId, string failureReason)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        FailureReason = failureReason;
    }

    public Guid PaymentId { get; }
    public Guid OrderId { get; }
    public string FailureReason { get; }
}
