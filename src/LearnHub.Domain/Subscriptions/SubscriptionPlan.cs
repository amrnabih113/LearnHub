using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.ValueObjects;

namespace LearnHub.Domain.Subscriptions;

public sealed class SubscriptionPlan : AuditableEntity
{
    public string Name { get; private set; } = default!;
    public SubscriptionTier Tier { get; private set; }
    public BillingCycle BillingCycle { get; private set; }
    public Money Price { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private SubscriptionPlan() { }

    private SubscriptionPlan(Guid id, string name, SubscriptionTier tier, BillingCycle billingCycle, Money price) : base(id)
    {
        Name = name;
        Tier = tier;
        BillingCycle = billingCycle;
        Price = price;
        IsActive = true;
    }

    public static Result<SubscriptionPlan> Create(Guid id, string name, SubscriptionTier tier, BillingCycle billingCycle, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return SubscriptionErrors.NameRequired;
        }

        if (!Enum.IsDefined(typeof(SubscriptionTier), tier))
        {
            return SubscriptionErrors.InvalidTier;
        }

        if (!Enum.IsDefined(typeof(BillingCycle), billingCycle))
        {
            return SubscriptionErrors.InvalidBillingCycle;
        }

        return new SubscriptionPlan(id, name.Trim(), tier, billingCycle, price);
    }

    public Result<Updated> Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }
}