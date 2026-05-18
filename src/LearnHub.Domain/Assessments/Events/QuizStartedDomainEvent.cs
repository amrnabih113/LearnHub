using LearnHub.Domain.Common;

namespace LearnHub.Domain.Assessments.Events;

public sealed class QuizStartedDomainEvent : DomainEvent
{
    public QuizStartedDomainEvent(Guid attemptId, Guid quizId, string studentId)
    {
        AttemptId = attemptId;
        QuizId = quizId;
        StudentId = studentId;
    }

    public Guid AttemptId { get; }
    public Guid QuizId { get; }
    public string StudentId { get; }
}
