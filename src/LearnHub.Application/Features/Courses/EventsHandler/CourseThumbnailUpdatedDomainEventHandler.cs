using LearnHub.Domain.Courses.Events;
using MediatR;

namespace LearnHub.Application.Features.Courses.Events;

public sealed class CourseThumbnailUpdatedDomainEventHandler : INotificationHandler<CourseThumbnailUpdatedDomainEvent>
{
    public Task Handle(CourseThumbnailUpdatedDomainEvent notification, CancellationToken cancellationToken)
        => Task.CompletedTask;
}