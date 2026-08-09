using LearnHub.Domain.Enrollments.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Enrollments.Events;

public sealed class EnrollmentCreatedDomainEventHandler(
    ILogger<EnrollmentCreatedDomainEventHandler> logger)
    : INotificationHandler<EnrollmentCreatedDomainEvent>
{
    private readonly ILogger<EnrollmentCreatedDomainEventHandler> _logger = logger;

    public Task Handle(EnrollmentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Enrollment {EnrollmentId} created for Student {StudentId} in Course {CourseId}.",
            notification.EnrollmentId,
            notification.StudentId,
            notification.CourseId);

        return Task.CompletedTask;
    }
}
