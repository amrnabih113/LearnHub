using LearnHub.Domain.Purchasing.Enums;

namespace LearnHub.Domain.Purchasing.ValueObjects;

public sealed record CouponSnapshot(
    CouponCode Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    string Currency,
    DateTimeOffset? ExpiresAtUtc,
    bool IsTemporaryFreeVoucher)
{
    public bool IsExpired(DateTimeOffset nowUtc)
        => ExpiresAtUtc.HasValue && nowUtc >= ExpiresAtUtc.Value;
}
