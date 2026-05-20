using LearnHub.Domain.Common;

namespace LearnHub.Domain.Purchasing.Events;

public sealed class OrderCheckedOutDomainEvent : DomainEvent
{
    public OrderCheckedOutDomainEvent(Guid orderId, Guid studentId, decimal totalAmount, string currency)
    {
        OrderId = orderId;
        StudentId = studentId;
        TotalAmount = totalAmount;
        Currency = currency;
    }

    public Guid OrderId { get; }
    public Guid StudentId { get; }
    public decimal TotalAmount { get; }
    public string Currency { get; }
}
