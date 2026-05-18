using LearnHub.Domain.Common;

namespace LearnHub.Domain.Assessments.Events;

public sealed class QuizPassedDomainEvent : DomainEvent
{
    public QuizPassedDomainEvent(Guid attemptId, Guid quizId, string studentId, decimal scorePercentage)
    {
        AttemptId = attemptId;
        QuizId = quizId;
        StudentId = studentId;
        ScorePercentage = scorePercentage;
    }

    public Guid AttemptId { get; }
    public Guid QuizId { get; }
    public string StudentId { get; }
    public decimal ScorePercentage { get; }
}
