using LearnHub.Domain.Common;

namespace LearnHub.Domain.Courses.Sections.Lessons.Events;

public sealed class LessonCreatedDomainEvent(Guid lessonId, Guid courseId) : DomainEvent
{
    public Guid LessonId { get; } = lessonId;

    public Guid CourseId { get; } = courseId;
}