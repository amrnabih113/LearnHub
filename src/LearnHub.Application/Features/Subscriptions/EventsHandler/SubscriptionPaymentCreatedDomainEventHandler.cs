using LearnHub.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Subscriptions.Events;

public sealed class SubscriptionPaymentCreatedDomainEventHandler(
    ILogger<SubscriptionPaymentCreatedDomainEventHandler> logger)
    : INotificationHandler<SubscriptionPaymentCreatedEvent>
{
    private readonly ILogger<SubscriptionPaymentCreatedDomainEventHandler> _logger = logger;

    public Task Handle(SubscriptionPaymentCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Subscription Payment {PaymentId} created for Subscription {SubscriptionId} (Amount: {Amount} {Currency}, DueAt: {DueAtUtc}).",
            notification.PaymentId,
            notification.SubscriptionId,
            notification.Amount,
            notification.Currency,
            notification.DueAtUtc);

        return Task.CompletedTask;
    }
}
