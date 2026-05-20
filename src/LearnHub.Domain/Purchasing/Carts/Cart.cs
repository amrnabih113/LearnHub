using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Purchasing.Carts;

public sealed class Cart : AuditableEntity
{
    public Guid StudentId { get; private set; }

    public string Currency { get; private set; } = null!;

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { }

    private Cart(Guid id, Guid studentId, string currency) : base(id)
    {
        StudentId = studentId;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public static Result<Cart> Create(Guid id, Guid studentId, string currency)
    {
        if (studentId == Guid.Empty)
        {
            return CartErrors.StudentIdRequired;
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
        {
            return CartErrors.InvalidCurrency;
        }

        return new Cart(id, studentId, currency);
    }

    public Result<Updated> AddItem(Guid courseId, string courseTitle, Money unitPrice)
    {
        if (courseId == Guid.Empty)
        {
            return CartErrors.CourseIdRequired;
        }

        if (string.IsNullOrWhiteSpace(courseTitle))
        {
            return CartErrors.CourseTitleRequired;
        }

        if (!string.Equals(unitPrice.Currency, Currency, StringComparison.OrdinalIgnoreCase))
        {
            return CartErrors.InvalidCurrency;
        }

        var existing = _items.FirstOrDefault(i => i.CourseId == courseId);
        if (existing is not null)
        {
            return CartErrors.ItemAlreadyAdded;
        }

        var createResult = CartItem.Create(Guid.NewGuid(), courseId, courseTitle, unitPrice);
        if (createResult.IsError)
        {
            return createResult.Errors;
        }

        _items.Add(createResult.Value);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> RemoveItem(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            return CartErrors.CourseIdRequired;
        }

        var removed = _items.RemoveAll(i => i.CourseId == courseId);
        if (removed == 0)
        {
            return CartErrors.ItemNotFound;
        }

        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }



    public Result<Money> GetTotal()
    {
        var total = Money.Zero(Currency);
        foreach (var item in _items)
        {
            var itemTotal = item.Total();
            var addResult = total.Add(itemTotal);
            if (addResult.IsError)
            {
                return addResult.Errors;
            }

            total = addResult.Value;
        }

        return total;
    }

    public Result<Updated> Clear()
    {
        _items.Clear();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }
}
