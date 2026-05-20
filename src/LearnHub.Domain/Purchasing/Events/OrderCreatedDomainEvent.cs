using LearnHub.Domain.Common;

namespace LearnHub.Domain.Purchasing.Events;

public sealed class OrderCreatedDomainEvent : DomainEvent
{
    public OrderCreatedDomainEvent(Guid orderId, Guid studentId)
    {
        OrderId = orderId;
        StudentId = studentId;
    }

    public Guid OrderId { get; }
    public Guid StudentId { get; }
}
