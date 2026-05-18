using LearnHub.Domain.Common;

namespace LearnHub.Domain.Purchasing.Events;

public sealed class OrderCreatedDomainEvent : DomainEvent
{
    public OrderCreatedDomainEvent(Guid orderId, string studentId)
    {
        OrderId = orderId;
        StudentId = studentId;
    }

    public Guid OrderId { get; }
    public string StudentId { get; }
}
