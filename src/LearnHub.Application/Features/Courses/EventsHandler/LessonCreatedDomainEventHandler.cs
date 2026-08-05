using LearnHub.Domain.Courses.Sections.Lessons.Events;
using MediatR;

namespace LearnHub.Application.Features.Courses.Events;

public sealed class LessonCreatedDomainEventHandler : INotificationHandler<LessonCreatedDomainEvent>
{
    public Task Handle(LessonCreatedDomainEvent notification, CancellationToken cancellationToken)
        => Task.CompletedTask;
}