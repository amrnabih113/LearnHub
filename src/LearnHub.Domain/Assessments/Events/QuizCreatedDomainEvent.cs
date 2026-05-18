using LearnHub.Domain.Common;

namespace LearnHub.Domain.Assessments.Events;

public sealed class QuizCreatedDomainEvent : DomainEvent
{
    public QuizCreatedDomainEvent(Guid quizId, Guid courseId)
    {
        QuizId = quizId;
        CourseId = courseId;
    }

    public Guid QuizId { get; }
    public Guid CourseId { get; }
}
