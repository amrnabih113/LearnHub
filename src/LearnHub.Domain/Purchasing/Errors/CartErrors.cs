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
}
