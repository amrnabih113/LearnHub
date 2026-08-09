using LearnHub.Domain.Purchasing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Purchasing.Events;

public sealed class RefundIssuedDomainEventHandler(
    ILogger<RefundIssuedDomainEventHandler> logger)
    : INotificationHandler<RefundIssuedDomainEvent>
{
    private readonly ILogger<RefundIssuedDomainEventHandler> _logger = logger;

    public Task Handle(RefundIssuedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Refund {PaymentId} issued for Order {OrderId} (Reason: {Reason}).",
            notification.PaymentId,
            notification.OrderId,
            notification.Reason);

        return Task.CompletedTask;
    }
}
