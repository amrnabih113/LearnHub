using LearnHub.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Subscriptions.Events;

public sealed class SubscriptionPaymentSucceededDomainEventHandler(
    ILogger<SubscriptionPaymentSucceededDomainEventHandler> logger)
    : INotificationHandler<SubscriptionPaymentSucceededEvent>
{
    private readonly ILogger<SubscriptionPaymentSucceededDomainEventHandler> _logger = logger;

    public Task Handle(SubscriptionPaymentSucceededEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Subscription Payment {PaymentId} succeeded for Subscription {SubscriptionId} (TransactionId: {GatewayTransactionId}, Amount: {Amount} {Currency}).",
            notification.PaymentId,
            notification.SubscriptionId,
            notification.GatewayTransactionId,
            notification.Amount,
            notification.Currency);

        return Task.CompletedTask;
    }
}
