using LearnHub.Domain.Common;

namespace LearnHub.Domain.Courses.Sections.Lessons.Events;

public sealed class LessonAddedDomainEvent : DomainEvent
{
    public LessonAddedDomainEvent(Guid lessonId, Guid sectionId, Guid courseId, string? title)
    {
        LessonId = lessonId;
        SectionId = sectionId;
        CourseId = courseId;
        Title = title;
    }

    public Guid LessonId { get; }
    public Guid SectionId { get; }
    public Guid CourseId { get; }
    public string? Title { get; }
}
