using LearnHub.Domain.Common;

namespace LearnHub.Domain.Courses.Events;

public sealed class CourseCreatedDomainEvent(Guid courseId, Guid instructorId) : DomainEvent
{
    public Guid CourseId { get; } = courseId;

    public Guid InstructorId { get; } = instructorId;
}