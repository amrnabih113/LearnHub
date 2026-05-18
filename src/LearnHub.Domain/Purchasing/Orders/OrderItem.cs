using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Purchasing.Orders;

public sealed class OrderItem : AuditableEntity
{
    public Guid CourseId { get; private set; }
    public string CourseTitle { get; private set; } = default!;
    public Money UnitPriceSnapshot { get; private set; } = default!;
    public int Quantity { get; private set; }

    private OrderItem() { }

    private OrderItem(Guid id, Guid courseId, string courseTitle, Money unitPriceSnapshot, int quantity) : base(id)
    {
        CourseId = courseId;
        CourseTitle = courseTitle;
        UnitPriceSnapshot = unitPriceSnapshot;
        Quantity = quantity;
    }

    public static Result<OrderItem> Create(Guid id, Guid courseId, string courseTitle, Money unitPriceSnapshot, int quantity)
    {
        if (courseId == Guid.Empty)
        {
            return OrderErrors.ItemNotFound;
        }

        if (string.IsNullOrWhiteSpace(courseTitle))
        {
            return OrderErrors.ItemNotFound;
        }

        if (quantity <= 0)
        {
            return OrderErrors.InvalidDiscount;
        }

        return new OrderItem(id, courseId, courseTitle.Trim(), unitPriceSnapshot, quantity);
    }

    public Result<Updated> UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            return OrderErrors.InvalidDiscount;
        }

        Quantity = quantity;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Money LineTotal => UnitPriceSnapshot.Multiply(Quantity);
}
