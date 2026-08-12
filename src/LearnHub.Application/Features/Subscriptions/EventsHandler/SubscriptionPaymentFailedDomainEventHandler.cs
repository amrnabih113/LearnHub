using LearnHub.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Subscriptions.Events;

public sealed class SubscriptionPaymentFailedDomainEventHandler(
    ILogger<SubscriptionPaymentFailedDomainEventHandler> logger)
    : INotificationHandler<SubscriptionPaymentFailedEvent>
{
    private readonly ILogger<SubscriptionPaymentFailedDomainEventHandler> _logger = logger;

    public Task Handle(SubscriptionPaymentFailedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Subscription Payment {PaymentId} failed for Subscription {SubscriptionId} (Attempt {AttemptCount}, Reason: {Reason}).",
            notification.PaymentId,
            notification.SubscriptionId,
            notification.AttemptCount,
            notification.Reason ?? "Unknown");

        return Task.CompletedTask;
    }
}
