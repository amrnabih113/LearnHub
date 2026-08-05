using LearnHub.Domain.Common;

namespace LearnHub.Domain.Courses.Events;

public sealed class CourseUpdatedDomainEvent(Guid courseId) : DomainEvent
{
    public Guid CourseId { get; } = courseId;
}