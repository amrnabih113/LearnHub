using LearnHub.Domain.Common;

namespace LearnHub.Domain.Purchasing.Events;

public sealed class RefundIssuedDomainEvent : DomainEvent
{
    public RefundIssuedDomainEvent(Guid paymentId, Guid orderId, string reason)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Reason = reason;
    }

    public Guid PaymentId { get; }
    public Guid OrderId { get; }
    public string Reason { get; }
}
