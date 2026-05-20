using LearnHub.Domain.Common;

namespace LearnHub.Domain.Assessments.Events;

public sealed class QuizStartedDomainEvent : DomainEvent
{
    public QuizStartedDomainEvent(Guid attemptId, Guid quizId, Guid studentId)
    {
        AttemptId = attemptId;
        QuizId = quizId;
        StudentId = studentId;
    }

    public Guid AttemptId { get; }
    public Guid QuizId { get; }
    public Guid StudentId { get; }
}
