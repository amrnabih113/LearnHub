using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Subscriptions.Events;

public sealed class SubscriptionActivatedDomainEventHandler(
    ICourseAccessService courseAccessService,
    ILogger<SubscriptionActivatedDomainEventHandler> logger)
    : INotificationHandler<SubscriptionActivatedEvent>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;
    private readonly ILogger<SubscriptionActivatedDomainEventHandler> _logger = logger;

    public async Task Handle(SubscriptionActivatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Subscription {SubscriptionId} activated for Student {StudentId} (Tier: {Tier}). Synchronizing enrollments...",
            notification.SubscriptionId,
            notification.StudentId,
            notification.Tier);

        await _courseAccessService.SynchronizeUserEnrollmentsAsync(
            notification.StudentId,
            cancellationToken);
    }
}
