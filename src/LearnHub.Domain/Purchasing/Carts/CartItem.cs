using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Purchasing.Carts;

public sealed class CartItem : AuditableEntity
{
    public Guid CourseId { get; private set; }

    public Guid CartId { get; private set; }
    public Cart Cart { get; private set; } = null!;

    public string CourseTitle { get; private set; } = null!;

    public Money UnitPrice { get; private set; } = null!;

    private CartItem() { }

    private CartItem(Guid id, Guid courseId, string courseTitle, Money unitPrice) : base(id)
    {
        CourseId = courseId;
        CourseTitle = courseTitle.Trim();
        UnitPrice = unitPrice;
    }

    public static Result<CartItem> Create(Guid id, Guid courseId, string courseTitle, Money unitPrice)
    {
        if (courseId == Guid.Empty)
        {
            return CartErrors.CourseIdRequired;
        }

        if (string.IsNullOrWhiteSpace(courseTitle))
        {
            return CartErrors.CourseTitleRequired;
        }

        return new CartItem(id, courseId, courseTitle, unitPrice);
    }

    public Money Total() => UnitPrice;
}
