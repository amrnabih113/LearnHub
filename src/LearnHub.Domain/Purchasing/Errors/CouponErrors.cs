using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Purchasing;

public static class CouponErrors
{
    public static Error CodeRequired
    => Error.Validation(code: "DomainError.Coupon.CodeRequired",
    description: "Coupon code is required");

    public static Error DiscountTypeRequired
    => Error.Validation(code: "DomainError.Coupon.DiscountTypeRequired",
    description: "Discount type is required");

    public static Error DiscountValueInvalid
    => Error.Validation(code: "DomainError.Coupon.DiscountValueInvalid",
    description: "Discount value is invalid");

    public static Error CurrencyRequired
    => Error.Validation(code: "DomainError.Coupon.CurrencyRequired",
    description: "Currency is required for fixed amount coupons");

    public static Error Expired
    => Error.Validation(code: "DomainError.Coupon.Expired",
    description: "Coupon has expired");

    public static Error Inactive
    => Error.Conflict(code: "DomainError.Coupon.Inactive",
    description: "Coupon is not active");

    public static Error RedemptionLimitReached
    => Error.Conflict(code: "DomainError.Coupon.RedemptionLimitReached",
    description: "Coupon redemption limit has been reached");

    public static Error CourseNotAllowed
    => Error.Conflict(code: "DomainError.Coupon.CourseNotAllowed",
    description: "Coupon cannot be used for this course");
}
