using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Grades;

public sealed class Grade : AuditableEntity
{
    public decimal ScorePercentage { get; private set; }
    public bool IsPassed { get; private set; }

    private Grade() { }

    private Grade(Guid id, decimal scorePercentage, bool isPassed) : base(id)
    {
        ScorePercentage = scorePercentage;
        IsPassed = isPassed;
    }

    public static Result<Grade> Create(Guid id, decimal scorePercentage, int passPercentage)
    {
        if (scorePercentage is < 0m or > 100m)
        {
            return GradeErrors.InvalidScore;
        }

        return new Grade(id, scorePercentage, scorePercentage >= passPercentage);
    }
}
