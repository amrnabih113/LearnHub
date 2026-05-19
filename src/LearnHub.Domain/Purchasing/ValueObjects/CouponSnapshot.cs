using LearnHub.Domain.Purchasing.Enums;

namespace LearnHub.Domain.Purchasing.ValueObjects;

public sealed record CouponSnapshot(
    string Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    string Currency,
    DateTimeOffset? ExpiresAtUtc
   )
{
    public bool IsExpired(DateTimeOffset nowUtc)
        => ExpiresAtUtc.HasValue && nowUtc >= ExpiresAtUtc.Value;
}
