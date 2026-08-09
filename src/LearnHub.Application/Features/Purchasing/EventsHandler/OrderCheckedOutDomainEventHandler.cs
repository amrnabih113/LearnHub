using LearnHub.Domain.Purchasing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Purchasing.Events;

public sealed class OrderCheckedOutDomainEventHandler(
    ILogger<OrderCheckedOutDomainEventHandler> logger)
    : INotificationHandler<OrderCheckedOutDomainEvent>
{
    private readonly ILogger<OrderCheckedOutDomainEventHandler> _logger = logger;

    public Task Handle(OrderCheckedOutDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Order {OrderId} checked out by Student {StudentId} (Total: {TotalAmount} {Currency}).",
            notification.OrderId,
            notification.StudentId,
            notification.TotalAmount,
            notification.Currency);

        return Task.CompletedTask;
    }
}
