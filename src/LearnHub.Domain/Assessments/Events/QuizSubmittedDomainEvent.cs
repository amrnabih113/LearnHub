using LearnHub.Domain.Common;

namespace LearnHub.Domain.Assessments.Events;

public sealed class QuizSubmittedDomainEvent : DomainEvent
{
    public QuizSubmittedDomainEvent(Guid attemptId, Guid quizId, string studentId, decimal scorePercentage, bool passed)
    {
        AttemptId = attemptId;
        QuizId = quizId;
        StudentId = studentId;
        ScorePercentage = scorePercentage;
        Passed = passed;
    }

    public Guid AttemptId { get; }
    public Guid QuizId { get; }
    public string StudentId { get; }
    public decimal ScorePercentage { get; }
    public bool Passed { get; }
}
