using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Purchasing;

public static class OrderErrors
{
    public static Error StudentIdRequired
    => Error.Validation(code: "DomainError.Order.StudentIdRequired",
    description: "Student id is required");

    public static Error CurrencyRequired
    => Error.Validation(code: "DomainError.Order.CurrencyRequired",
    description: "Currency is required");

    public static Error EmptyOrder
    => Error.Validation(code: "DomainError.Order.EmptyOrder",
    description: "Cannot checkout an empty order");

    public static Error ItemAlreadyExists
    => Error.Conflict(code: "DomainError.Order.ItemAlreadyExists",
    description: "Course is already present in the order");

    public static Error ItemNotFound
    => Error.NotFound(code: "DomainError.Order.ItemNotFound",
    description: "Order item was not found");

    public static Error NotDraft
    => Error.Conflict(code: "DomainError.Order.NotDraft",
    description: "Only draft orders can be modified");

    public static Error AlreadyCheckedOut
    => Error.Conflict(code: "DomainError.Order.AlreadyCheckedOut",
    description: "Order is already checked out");

    public static Error AlreadyPaid
    => Error.Conflict(code: "DomainError.Order.AlreadyPaid",
    description: "Order is already paid");

    public static Error PaymentRequired
    => Error.Conflict(code: "DomainError.Order.PaymentRequired",
    description: "Order requires payment before it can be marked paid");

    public static Error InvalidCurrency
    => Error.Validation(code: "DomainError.Order.InvalidCurrency",
    description: "Currencies must match across order items and discounts");

    public static Error CouponExpired
    => Error.Validation(code: "DomainError.Order.CouponExpired",
    description: "Coupon has expired");

    public static Error CouponRequired
    => Error.Validation(code: "DomainError.Order.CouponRequired",
    description: "Coupon code is required");

    public static Error InvalidDiscount
    => Error.Validation(code: "DomainError.Order.InvalidDiscount",
    description: "Discount value is invalid");

    public static Error CannotRefund
    => Error.Conflict(code: "DomainError.Order.CannotRefund",
    description: "Only paid orders can be refunded");
}
