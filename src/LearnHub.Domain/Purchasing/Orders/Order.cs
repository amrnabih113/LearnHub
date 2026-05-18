using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Purchasing.Events;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Purchasing.Orders;

public sealed class Order : AuditableEntity
{
    public string StudentId { get; private set; } = default!;
    public string Currency { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public CouponSnapshot? AppliedCoupon { get; private set; }
    public Money SubtotalAmount { get; private set; } = default!;
    public Money DiscountAmount { get; private set; } = default!;
    public Money TotalAmount { get; private set; } = default!;
    public DateTimeOffset? CheckedOutAtUtc { get; private set; }
    public DateTimeOffset? PaidAtUtc { get; private set; }
    public DateTimeOffset? FailedAtUtc { get; private set; }
    public DateTimeOffset? RefundedAtUtc { get; private set; }
    public string? FailureReason { get; private set; }
    public string? RefundReason { get; private set; }
    public TransactionId? TransactionId { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    private Order(Guid id, string studentId, string currency) : base(id)
    {
        StudentId = studentId;
        Currency = currency;
        Status = OrderStatus.Draft;
        SubtotalAmount = Money.Zero(currency);
        DiscountAmount = Money.Zero(currency);
        TotalAmount = Money.Zero(currency);
    }

    public static Result<Order> Create(Guid id, string studentId, string currency)
    {
        if (string.IsNullOrWhiteSpace(studentId))
        {
            return OrderErrors.StudentIdRequired;
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            return OrderErrors.CurrencyRequired;
        }

        var order = new Order(id, studentId.Trim(), currency.Trim().ToUpperInvariant());
        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, order.StudentId));

        return order;
    }

    public Result<Updated> AddItem(Guid courseId, string courseTitle, Money coursePriceSnapshot, int quantity = 1)
    {
        if (Status != OrderStatus.Draft)
        {
            return OrderErrors.NotDraft;
        }

        if (courseId == Guid.Empty)
        {
            return OrderErrors.ItemNotFound;
        }

        if (_items.Any(item => item.CourseId == courseId))
        {
            return OrderErrors.ItemAlreadyExists;
        }

        if (!string.Equals(coursePriceSnapshot.Currency, Currency, StringComparison.OrdinalIgnoreCase))
        {
            return OrderErrors.InvalidCurrency;
        }

        var createResult = OrderItem.Create(Guid.NewGuid(), courseId, courseTitle, coursePriceSnapshot, quantity);
        if (createResult.IsError)
        {
            return createResult.Errors;
        }

        _items.Add(createResult.Value);
        RecalculateTotals();
        return Result.Updated;
    }

    public Result<Updated> RemoveItem(Guid courseId)
    {
        if (Status != OrderStatus.Draft)
        {
            return OrderErrors.NotDraft;
        }

        var removed = _items.RemoveAll(item => item.CourseId == courseId);
        if (removed == 0)
        {
            return OrderErrors.ItemNotFound;
        }

        if (_items.Count == 0)
        {
            AppliedCoupon = null;
        }

        RecalculateTotals();
        return Result.Updated;
    }

    public Result<Updated> ApplyCoupon(CouponSnapshot couponSnapshot)
    {
        if (Status != OrderStatus.Draft)
        {
            return OrderErrors.NotDraft;
        }

        if (couponSnapshot.IsExpired(DateTimeOffset.UtcNow))
        {
            return OrderErrors.CouponExpired;
        }

        if (!string.Equals(couponSnapshot.Currency, Currency, StringComparison.OrdinalIgnoreCase))
        {
            return OrderErrors.InvalidCurrency;
        }

        if (couponSnapshot.DiscountValue <= 0)
        {
            return OrderErrors.InvalidDiscount;
        }

        AppliedCoupon = couponSnapshot;
        RecalculateTotals();
        return Result.Updated;
    }

    public Result<Updated> Checkout(DateTimeOffset checkedOutAtUtc)
    {
        if (_items.Count == 0)
        {
            return OrderErrors.EmptyOrder;
        }

        if (Status != OrderStatus.Draft)
        {
            return OrderErrors.AlreadyCheckedOut;
        }

        RecalculateTotals();
        CheckedOutAtUtc = checkedOutAtUtc;

        if (TotalAmount.Amount == 0)
        {
            Status = OrderStatus.Paid;
            PaidAtUtc = checkedOutAtUtc;
        }
        else
        {
            Status = OrderStatus.PendingPayment;
        }

        UpdatedAtUtc = checkedOutAtUtc;
        AddDomainEvent(new OrderCheckedOutDomainEvent(Id, StudentId, TotalAmount.Amount, Currency));
        return Result.Updated;
    }

    public Result<Updated> MarkPaid(TransactionId transactionId, DateTimeOffset paidAtUtc)
    {
        if (Status == OrderStatus.Paid)
        {
            return OrderErrors.AlreadyPaid;
        }

        if (Status != OrderStatus.PendingPayment)
        {
            return OrderErrors.PaymentRequired;
        }

        TransactionId = transactionId;
        Status = OrderStatus.Paid;
        PaidAtUtc = paidAtUtc;
        UpdatedAtUtc = paidAtUtc;
        return Result.Updated;
    }

    public Result<Updated> MarkFailed(string failureReason, DateTimeOffset failedAtUtc)
    {
        if (Status != OrderStatus.PendingPayment)
        {
            return OrderErrors.PaymentRequired;
        }

        Status = OrderStatus.Failed;
        FailureReason = failureReason;
        FailedAtUtc = failedAtUtc;
        UpdatedAtUtc = failedAtUtc;
        return Result.Updated;
    }

    public Result<Updated> Refund(string reason, DateTimeOffset refundedAtUtc)
    {
        if (Status != OrderStatus.Paid)
        {
            return OrderErrors.CannotRefund;
        }

        Status = OrderStatus.Refunded;
        RefundReason = reason;
        RefundedAtUtc = refundedAtUtc;
        UpdatedAtUtc = refundedAtUtc;
        return Result.Updated;
    }

    private void RecalculateTotals()
    {
        var subtotal = Money.Zero(Currency);
        foreach (var item in _items)
        {
            subtotal = subtotal.Add(item.LineTotal).Value;
        }

        var discount = CalculateDiscount(subtotal);
        var total = subtotal.Subtract(discount).Value;

        SubtotalAmount = subtotal;
        DiscountAmount = discount;
        TotalAmount = total;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    private Money CalculateDiscount(Money subtotal)
    {
        if (AppliedCoupon is null)
        {
            return Money.Zero(Currency);
        }

        if (!string.Equals(AppliedCoupon.Currency, Currency, StringComparison.OrdinalIgnoreCase))
        {
            return Money.Zero(Currency);
        }

        return AppliedCoupon.DiscountType switch
        {
            DiscountType.Percentage => Money.Create(subtotal.Amount * AppliedCoupon.DiscountValue / 100m, Currency).Value,
            DiscountType.FixedAmount => Money.Create(Math.Min(subtotal.Amount, AppliedCoupon.DiscountValue), Currency).Value,
            _ => Money.Zero(Currency)
        };
    }
}
