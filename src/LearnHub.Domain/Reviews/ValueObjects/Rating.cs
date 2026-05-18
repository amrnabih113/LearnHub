using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Reviews.ValueObjects;

public sealed record Rating
{
    public int Value { get; }

    private Rating(int value)
    {
        Value = value;
    }

    public static Result<Rating> Create(int value)
    {
        if (value < 1 || value > 5)
        {
            return ReviewErrors.RatingInvalid;
        }

        return new Rating(value);
    }
}
