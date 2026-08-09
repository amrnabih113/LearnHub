using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Subscriptions.Events;

public sealed class SubscriptionExpiredDomainEventHandler(
    ICourseAccessService courseAccessService,
    ILogger<SubscriptionExpiredDomainEventHandler> logger)
    : INotificationHandler<SubscriptionExpiredEvent>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;
    private readonly ILogger<SubscriptionExpiredDomainEventHandler> _logger = logger;

    public async Task Handle(SubscriptionExpiredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Subscription {SubscriptionId} expired for Student {StudentId} at {ExpiredAtUtc}. Re-evaluating course access...",
            notification.SubscriptionId,
            notification.StudentId,
            notification.ExpiredAtUtc);

        await _courseAccessService.SynchronizeUserEnrollmentsAsync(
            notification.StudentId,
            cancellationToken);
    }
}
