using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Purchasing.Carts;

public sealed class CartItem : AuditableEntity
{
    public Guid CourseId { get; private set; }

    public string CourseTitle { get; private set; } = null!;

    public Money UnitPrice { get; private set; } = null!;

    public int Quantity { get; private set; }

    private CartItem() { }

    private CartItem(Guid id, Guid courseId, string courseTitle, Money unitPrice, int quantity) : base(id)
    {
        CourseId = courseId;
        CourseTitle = courseTitle.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static Result<CartItem> Create(Guid id, Guid courseId, string courseTitle, Money unitPrice, int quantity)
    {
        if (courseId == Guid.Empty)
        {
            return CartErrors.CourseIdRequired;
        }

        if (string.IsNullOrWhiteSpace(courseTitle))
        {
            return CartErrors.CourseTitleRequired;
        }

        if (quantity <= 0)
        {
            return CartErrors.QuantityInvalid;
        }

        return new CartItem(id, courseId, courseTitle, unitPrice, quantity);
    }

    public Result<Updated> UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            return CartErrors.QuantityInvalid;
        }

        Quantity = quantity;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Money Total() => UnitPrice.Multiply(Quantity);
}
