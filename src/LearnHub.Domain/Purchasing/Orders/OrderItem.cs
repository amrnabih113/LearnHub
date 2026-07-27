using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Purchasing.Orders;

public sealed class OrderItem : AuditableEntity
{
    public Guid CourseId { get; private set; }

    public Guid OrderId { get; private set; }

    public Order Order { get; private set; } = default!;
    public string CourseTitle { get; private set; } = default!;
    public Money UnitPriceSnapshot { get; private set; } = default!;

    private OrderItem() { }

    private OrderItem(Guid id, Guid courseId, string courseTitle, Money unitPriceSnapshot) : base(id)
    {
        CourseId = courseId;
        CourseTitle = courseTitle;
        UnitPriceSnapshot = unitPriceSnapshot;
    }

    public static Result<OrderItem> Create(Guid id, Guid courseId, string courseTitle, Money unitPriceSnapshot)
    {
        if (courseId == Guid.Empty)
        {
            return OrderErrors.ItemNotFound;
        }

        if (string.IsNullOrWhiteSpace(courseTitle))
        {
            return OrderErrors.ItemNotFound;
        }

        return new OrderItem(id, courseId, courseTitle.Trim(), unitPriceSnapshot);
    }

    public Money LineTotal => UnitPriceSnapshot;
}
