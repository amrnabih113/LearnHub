using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Subscriptions.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Subscriptions.Events;

public sealed class TrialStartedDomainEventHandler(
    ICourseAccessService courseAccessService,
    ILogger<TrialStartedDomainEventHandler> logger)
    : INotificationHandler<TrialStartedEvent>
{
    private readonly ICourseAccessService _courseAccessService = courseAccessService;
    private readonly ILogger<TrialStartedDomainEventHandler> _logger = logger;

    public async Task Handle(TrialStartedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Trial started for Subscription {SubscriptionId}, Student {StudentId} (Tier: {Tier}) until {TrialEndsAtUtc}.",
            notification.SubscriptionId,
            notification.StudentId,
            notification.Tier,
            notification.TrialEndsAtUtc);

        await _courseAccessService.SynchronizeUserEnrollmentsAsync(
            notification.StudentId,
            cancellationToken);
    }
}
