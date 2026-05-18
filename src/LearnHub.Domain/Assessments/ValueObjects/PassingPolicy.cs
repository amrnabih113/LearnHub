using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Assessments.ValueObjects;

public sealed record PassingPolicy
{
    public int MaxAttempts { get; }
    public int PassPercentage { get; }

    private PassingPolicy(int maxAttempts, int passPercentage)
    {
        MaxAttempts = maxAttempts;
        PassPercentage = passPercentage;
    }

    public static Result<PassingPolicy> Create(int maxAttempts, int passPercentage)
    {
        if (maxAttempts <= 0)
        {
            return QuizErrors.MaxAttemptsInvalid;
        }

        if (passPercentage is < 1 or > 100)
        {
            return QuizErrors.PassPercentageInvalid;
        }

        return new PassingPolicy(maxAttempts, passPercentage);
    }
}
