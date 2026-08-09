using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Subscriptions.Events;

public sealed class SubscriptionUpgradedDomainEventHandler(
    ICourseAccessService courseAccessService,
    ILogger<SubscriptionUpgradedDomainEventHandler> logger)
    : INotificationHandler<SubscriptionUpgradedEvent>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;
    private readonly ILogger<SubscriptionUpgradedDomainEventHandler> _logger = logger;

    public async Task Handle(SubscriptionUpgradedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Subscription {SubscriptionId} upgraded for Student {StudentId} from {OldTier} to {NewTier}.",
            notification.SubscriptionId,
            notification.StudentId,
            notification.OldTier,
            notification.NewTier);

        await _courseAccessService.SynchronizeUserEnrollmentsAsync(
            notification.StudentId,
            cancellationToken);
    }
}
