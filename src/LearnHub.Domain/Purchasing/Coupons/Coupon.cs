using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Purchasing.Coupons;

public sealed class Coupon : AuditableEntity
{
    public CouponCode Code { get; private set; } = default!;
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public string Currency { get; private set; } = default!;
    public DateTimeOffset? ExpiresAtUtc { get; private set; }
    public int? MaxRedemptions { get; private set; }
    public int RedemptionCount { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Guid> _allowedCourseIds = [];
    public IReadOnlyCollection<Guid> AllowedCourseIds => _allowedCourseIds.AsReadOnly();

    private Coupon() { }

    private Coupon(Guid id, CouponCode code, DiscountType discountType, decimal discountValue, string currency, DateTimeOffset? expiresAtUtc, int? maxRedemptions) : base(id)
    {
        Code = code;
        DiscountType = discountType;
        DiscountValue = discountValue;
        Currency = currency;
        ExpiresAtUtc = expiresAtUtc;
        MaxRedemptions = maxRedemptions;
        IsActive = true;
    }

    public static Result<Coupon> Create(Guid id, string code, DiscountType discountType, decimal discountValue, string currency, DateTimeOffset? expiresAtUtc = null, int? maxRedemptions = null, IEnumerable<Guid>? allowedCourseIds = null)
    {
        var codeResult = CouponCode.Create(code);
        if (codeResult.IsError)
        {
            return codeResult.Errors;
        }

        if (!Enum.IsDefined(typeof(DiscountType), discountType))
        {
            return CouponErrors.DiscountTypeRequired;
        }

        if (discountValue <= 0)
        {
            return CouponErrors.DiscountValueInvalid;
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            return CouponErrors.CurrencyRequired;
        }

        var normalizedCurrency = currency.Trim().ToUpperInvariant();
        if (normalizedCurrency.Length != 3)
        {
            return CouponErrors.CurrencyRequired;
        }

        if (discountType == DiscountType.Percentage && discountValue > 100)
        {
            return CouponErrors.DiscountValueInvalid;
        }

        if (expiresAtUtc.HasValue && expiresAtUtc.Value <= DateTimeOffset.UtcNow)
        {
            return CouponErrors.Expired;
        }

        var coupon = new Coupon(id, codeResult.Value, discountType, discountValue, normalizedCurrency, expiresAtUtc, maxRedemptions);
        if (allowedCourseIds is not null)
        {
            coupon._allowedCourseIds.AddRange(allowedCourseIds.Where(courseId => courseId != Guid.Empty));
        }

        return coupon;
    }

    public Result<Updated> Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Redeem(Guid courseId)
    {
        if (!IsActive)
        {
            return CouponErrors.Inactive;
        }

        if (ExpiresAtUtc.HasValue && DateTimeOffset.UtcNow >= ExpiresAtUtc.Value)
        {
            return CouponErrors.Expired;
        }

        if (MaxRedemptions.HasValue && RedemptionCount >= MaxRedemptions.Value)
        {
            return CouponErrors.RedemptionLimitReached;
        }

        if (_allowedCourseIds.Count > 0 && !_allowedCourseIds.Contains(courseId))
        {
            return CouponErrors.CourseNotAllowed;
        }

        RedemptionCount++;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public CouponSnapshot ToSnapshot(bool isTemporaryFreeVoucher = false)
    {
        return new CouponSnapshot(Code, DiscountType, DiscountValue, Currency, ExpiresAtUtc, isTemporaryFreeVoucher);
    }
}
