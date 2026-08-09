using LearnHub.Domain.Enrollments.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Enrollments.Events;

public sealed class CourseCompletedDomainEventHandler(
    ILogger<CourseCompletedDomainEventHandler> logger)
    : INotificationHandler<CourseCompletedDomainEvent>
{
    private readonly ILogger<CourseCompletedDomainEventHandler> _logger = logger;

    public Task Handle(CourseCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Course {CourseId} completed for Student {StudentId} (EnrollmentId: {EnrollmentId}).",
            notification.CourseId,
            notification.StudentId,
            notification.EnrollmentId);

        return Task.CompletedTask;
    }
}
