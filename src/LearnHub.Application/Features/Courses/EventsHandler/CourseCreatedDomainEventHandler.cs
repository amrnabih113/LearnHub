using LearnHub.Domain.Courses.Events;
using MediatR;

namespace LearnHub.Application.Features.Courses.Events;

public sealed class CourseCreatedDomainEventHandler : INotificationHandler<CourseCreatedDomainEvent>
{
    public Task Handle(CourseCreatedDomainEvent notification, CancellationToken cancellationToken)
        => Task.CompletedTask;
}