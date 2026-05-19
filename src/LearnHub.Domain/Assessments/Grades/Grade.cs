using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Grades;

public sealed class Grade : AuditableEntity
{
    public decimal Score { get; private set; }

    public decimal TotalScore { get; private set; }
    public decimal ScorePercentage { get; private set; }
    public bool IsPassed { get; private set; }

    private Grade() { }

    private Grade(Guid id, decimal score, decimal totalScore, decimal scorePercentage, bool isPassed) : base(id)
    {
        Score = score;
        TotalScore = totalScore;
        ScorePercentage = scorePercentage;
        IsPassed = isPassed;
    }

    public static Result<Grade> Create(Guid id, decimal score, decimal totalScore, int passPercentage)
    {
        if (score < 0m || totalScore < 0m)
        {
            return GradeErrors.InvalidScore;
        }

        if (totalScore > 0m && score > totalScore)
        {
            return GradeErrors.InvalidScore;
        }

        decimal percentage = 0m;
        if (totalScore > 0m)
        {
            percentage = Math.Round((score / totalScore) * 100m, 2);
        }

        if (percentage < 0m || percentage > 100m)
        {
            return GradeErrors.InvalidScore;
        }

        var passed = percentage >= passPercentage;
        return new Grade(id, score, totalScore, percentage, passed);
    }

    // Backwards-compatible overload: create from percentage (0-100).
    public static Result<Grade> Create(Guid id, decimal scorePercentage, int passPercentage)
    {
        if (scorePercentage is < 0m or > 100m)
        {
            return GradeErrors.InvalidScore;
        }

        var passed = scorePercentage >= passPercentage;
        // Represent percentage as score out of 100
        return new Grade(id, scorePercentage, 100m, scorePercentage, passed);
    }
}
