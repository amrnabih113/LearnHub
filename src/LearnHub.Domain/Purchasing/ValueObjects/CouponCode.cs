using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Purchasing.ValueObjects;

public sealed record CouponCode
{
    public string Value { get; }

    private CouponCode(string value)
    {
        Value = value;
    }

    public static Result<CouponCode> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return CouponErrors.CodeRequired;
        }

        return new CouponCode(value.Trim().ToUpperInvariant());
    }
}
