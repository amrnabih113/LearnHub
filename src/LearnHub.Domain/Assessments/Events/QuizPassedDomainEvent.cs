using LearnHub.Domain.Common;

namespace LearnHub.Domain.Assessments.Events;

public sealed class QuizPassedDomainEvent : DomainEvent
{
    public QuizPassedDomainEvent(Guid attemptId, Guid quizId, Guid studentId, decimal scorePercentage)
    {
        AttemptId = attemptId;
        QuizId = quizId;
        StudentId = studentId;
        ScorePercentage = scorePercentage;
    }

    public Guid AttemptId { get; }
    public Guid QuizId { get; }
    public Guid StudentId { get; }
    public decimal ScorePercentage { get; }
}
