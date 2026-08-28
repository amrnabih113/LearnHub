using LearnHub.Application.Features.Certificates.Commands.IssueCertificate;
using LearnHub.Domain.Enrollments.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Enrollments.Events;

public sealed class CourseCompletedDomainEventHandler(
    ISender sender,
    ILogger<CourseCompletedDomainEventHandler> logger)
    : INotificationHandler<CourseCompletedDomainEvent>
{
    private readonly ISender _sender = sender;
    private readonly ILogger<CourseCompletedDomainEventHandler> _logger = logger;

    public async Task Handle(CourseCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Course {CourseId} completed for Student {StudentId} (EnrollmentId: {EnrollmentId}). Issuing certificate...",
            notification.CourseId,
            notification.StudentId,
            notification.EnrollmentId);

        var result = await _sender.Send(new IssueCertificateCommand(notification.StudentId, notification.CourseId), cancellationToken);

        if (result.IsError)
        {
            _logger.LogWarning(
                "Automatic certificate issuance failed for Student {StudentId}, Course {CourseId}: {Error}",
                notification.StudentId,
                notification.CourseId,
                result.TopError.Description);
        }
        else
        {
            _logger.LogInformation(
                "Certificate {CertificateCode} issued successfully for Student {StudentId}, Course {CourseId}.",
                result.Value.Code,
                notification.StudentId,
                notification.CourseId);
        }
    }
}
