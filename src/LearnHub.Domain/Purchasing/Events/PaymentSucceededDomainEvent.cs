using LearnHub.Domain.Common;

namespace LearnHub.Domain.Purchasing.Events;

public sealed class PaymentSucceededDomainEvent : DomainEvent
{
    public PaymentSucceededDomainEvent(Guid paymentId, Guid orderId, string transactionId)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        TransactionId = transactionId;
    }

    public Guid PaymentId { get; }
    public Guid OrderId { get; }
    public string TransactionId { get; }
}
