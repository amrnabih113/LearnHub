using LearnHub.Domain.Purchasing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Purchasing.Events;

public sealed class PaymentFailedDomainEventHandler(
    ILogger<PaymentFailedDomainEventHandler> logger)
    : INotificationHandler<PaymentFailedDomainEvent>
{
    private readonly ILogger<PaymentFailedDomainEventHandler> _logger = logger;

    public Task Handle(PaymentFailedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Payment {PaymentId} failed for Order {OrderId} (Reason: {FailureReason}).",
            notification.PaymentId,
            notification.OrderId,
            notification.FailureReason);

        return Task.CompletedTask;
    }
}
