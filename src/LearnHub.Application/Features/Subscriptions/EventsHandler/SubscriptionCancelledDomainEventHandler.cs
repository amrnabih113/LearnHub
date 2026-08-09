using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Subscriptions.Events;

public sealed class SubscriptionCancelledDomainEventHandler(
    ICourseAccessService courseAccessService,
    ILogger<SubscriptionCancelledDomainEventHandler> logger)
    : INotificationHandler<SubscriptionCancelledEvent>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;
    private readonly ILogger<SubscriptionCancelledDomainEventHandler> _logger = logger;

    public async Task Handle(SubscriptionCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Subscription {SubscriptionId} cancelled for Student {StudentId} at {CancelledAtUtc}. Re-evaluating course access...",
            notification.SubscriptionId,
            notification.StudentId,
            notification.CancelledAtUtc);

        await _courseAccessService.SynchronizeUserEnrollmentsAsync(
            notification.StudentId,
            cancellationToken);
    }
}
