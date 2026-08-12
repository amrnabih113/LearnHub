using LearnHub.Domain.Purchasing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Purchasing.Events;

public sealed class OrderCreatedDomainEventHandler(
    ILogger<OrderCreatedDomainEventHandler> logger)
    : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly ILogger<OrderCreatedDomainEventHandler> _logger = logger;

    public Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Order {OrderId} created for Student {StudentId}.",
            notification.OrderId,
            notification.StudentId);

        return Task.CompletedTask;
    }
}
