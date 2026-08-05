using LearnHub.Domain.Courses.Events;
using MediatR;

namespace LearnHub.Application.Features.Courses.Events;

public sealed class CourseStatusChangedDomainEventHandler : INotificationHandler<CourseStatusChangedDomainEvent>
{
    public Task Handle(CourseStatusChangedDomainEvent notification, CancellationToken cancellationToken)
        => Task.CompletedTask;
}