using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Purchasing.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Purchasing.Events;

public sealed class PaymentSucceededDomainEventHandler(
    ICourseAccessService courseAccessService,
    ILogger<PaymentSucceededDomainEventHandler> logger)
    : INotificationHandler<PaymentSucceededDomainEvent>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;
    private readonly ILogger<PaymentSucceededDomainEventHandler> _logger = logger;

    public async Task Handle(PaymentSucceededDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Payment {PaymentId} succeeded for Order {OrderId} (TransactionId: {TransactionId}). Processing course enrollments...",
            notification.PaymentId,
            notification.OrderId,
            notification.TransactionId);

        await _courseAccessService.ProcessOrderPaymentSucceededAsync(
            notification.OrderId,
            cancellationToken);
    }
}
