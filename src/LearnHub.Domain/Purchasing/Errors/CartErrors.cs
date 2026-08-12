using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Purchasing;

public static class CartErrors
{
    public static Error StudentIdRequired
    => Error.Validation(code: "DomainError.Cart.StudentIdRequired",
    description: "Student id is required");

    public static Error InvalidCurrency
    => Error.Validation(code: "DomainError.Cart.InvalidCurrency",
    description: "Cart currency is invalid. Use 3-letter ISO currency code.");

    public static Error CourseIdRequired
    => Error.Validation(code: "DomainError.Cart.CourseIdRequired",
    description: "Course id is required");

    public static Error CourseTitleRequired
    => Error.Validation(code: "DomainError.Cart.CourseTitleRequired",
    description: "Course title is required");

    public static Error QuantityInvalid
    => Error.Validation(code: "DomainError.Cart.QuantityInvalid",
    description: "Quantity must be at least 1");

    public static Error ItemNotFound
    => Error.NotFound(code: "DomainError.Cart.ItemNotFound",
    description: "Cart item was not found");

    public static Error ItemAlreadyAdded
    => Error.Conflict(code: "DomainError.Cart.ItemAlreadyAdded",
    description: "Item is already present in the cart");

    public static Error CourseAlreadyEnrolled
    => Error.Conflict(code: "DomainError.Cart.CourseAlreadyEnrolled",
    description: "You are already enrolled in this course.");

    public static Error EmptyCart
    => Error.Validation(code: "DomainError.Cart.EmptyCart",
    description: "Cart is empty.");

    public static Error CouponCodeRequired
    => Error.Validation(code: "DomainError.Cart.CouponCodeRequired",
    description: "Coupon code is required.");

    public static Error CouponNotFound
    => Error.NotFound(code: "DomainError.Cart.CouponNotFound",
    description: "Coupon was not found.");

    public static Error CouponNotApplicable
    => Error.Validation(code: "DomainError.Cart.CouponNotApplicable",
    description: "Coupon is not applicable to any item in your cart.");
}
