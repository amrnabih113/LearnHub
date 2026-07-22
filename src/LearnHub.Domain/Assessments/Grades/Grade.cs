using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.Grades;

public sealed record Grade
{
    public decimal Score { get; init; }

    public decimal TotalScore { get; init; }

    public decimal ScorePercentage { get; init; }

    public bool IsPassed { get; init; }


    private Grade(
        decimal score,
        decimal totalScore,
        decimal scorePercentage,
        bool isPassed)
    {
        Score = score;
        TotalScore = totalScore;
        ScorePercentage = scorePercentage;
        IsPassed = isPassed;
    }


    public static Result<Grade> Create(
        decimal score,
        decimal totalScore,
        int passPercentage)
    {
        if (score < 0 || totalScore < 0)
        {
            return GradeErrors.InvalidScore;
        }

        if (totalScore > 0 && score > totalScore)
        {
            return GradeErrors.InvalidScore;
        }

        var percentage = totalScore == 0
            ? 0
            : Math.Round((score / totalScore) * 100m, 2);


        if (percentage < 0 || percentage > 100)
        {
            return GradeErrors.InvalidScore;
        }


        return new Grade(
            score,
            totalScore,
            percentage,
            percentage >= passPercentage);
    }


    public static Result<Grade> CreateFromPercentage(
        decimal scorePercentage,
        int passPercentage)
    {
        if (scorePercentage < 0 || scorePercentage > 100)
        {
            return GradeErrors.InvalidScore;
        }


        return new Grade(
            scorePercentage,
            100m,
            scorePercentage,
            scorePercentage >= passPercentage);
    }
}