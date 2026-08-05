using LearnHub.Domain.Courses.Events;
using MediatR;

namespace LearnHub.Application.Features.Courses.Events;

public sealed class CourseUpdatedDomainEventHandler : INotificationHandler<CourseUpdatedDomainEvent>
{
    public Task Handle(CourseUpdatedDomainEvent notification, CancellationToken cancellationToken)
        => Task.CompletedTask;
}