using LearnHub.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Subscriptions.Events;

public sealed class SubscriptionPaymentRefundedDomainEventHandler(
    ILogger<SubscriptionPaymentRefundedDomainEventHandler> logger)
    : INotificationHandler<SubscriptionPaymentRefundedEvent>
{
    private readonly ILogger<SubscriptionPaymentRefundedDomainEventHandler> _logger = logger;

    public Task Handle(SubscriptionPaymentRefundedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Subscription Payment {PaymentId} refunded for Subscription {SubscriptionId} (Reason: {RefundReason}).",
            notification.PaymentId,
            notification.SubscriptionId,
            notification.RefundReason ?? "No reason provided");

        return Task.CompletedTask;
    }
}
