using LearnHub.Domain.Common;

namespace LearnHub.Domain.Assessments.Events;

public sealed class QuizPublishedDomainEvent : DomainEvent
{
    public QuizPublishedDomainEvent(Guid quizId, Guid courseId)
    {
        QuizId = quizId;
        CourseId = courseId;
    }

    public Guid QuizId { get; }
    public Guid CourseId { get; }
}
