using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Subscriptions;

public sealed class Subscription : AuditableEntity
{
    public Guid StudentId { get; private set; }
    public SubscriptionTier Tier { get; private set; }
    public Guid SubscriptionPlanId { get; private set; }
    public SubscriptionPlan Plan { get; private set; } = default!;
    public BillingCycle BillingCycle { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTimeOffset StartedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? TrialEndsAtUtc { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }
    public bool AutoRenewEnabled { get; private set; }

    private readonly List<SubscriptionPayment> _payments = new();
    public IReadOnlyCollection<SubscriptionPayment> Payments => _payments.AsReadOnly();

    private Subscription() { }

    private Subscription(Guid id, Guid studentId, SubscriptionTier tier, BillingCycle billingCycle, DateTimeOffset startedAtUtc, DateTimeOffset expiresAtUtc) : base(id)
    {
        StudentId = studentId;
        Tier = tier;
        BillingCycle = billingCycle;
        StartedAtUtc = startedAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        Status = SubscriptionStatus.PendingActivation;
    }

    public static Result<Subscription> Create(Guid id, Guid studentId, SubscriptionTier tier, BillingCycle billingCycle, DateTimeOffset startedAtUtc, DateTimeOffset expiresAtUtc)
    {
        if (studentId == Guid.Empty)
        {
            return SubscriptionErrors.StudentIdRequired;
        }

        if (!Enum.IsDefined(typeof(SubscriptionTier), tier))
        {
            return SubscriptionErrors.InvalidTier;
        }

        if (!Enum.IsDefined(typeof(BillingCycle), billingCycle))
        {
            return SubscriptionErrors.InvalidBillingCycle;
        }

        if (expiresAtUtc <= startedAtUtc)
        {
            return SubscriptionErrors.ExpirationRequired;
        }

        return new Subscription(id, studentId, tier, billingCycle, startedAtUtc, expiresAtUtc);
    }

    public Result<Updated> Activate(DateTimeOffset activatedAtUtc)
    {
        if (Status is SubscriptionStatus.Cancelled or SubscriptionStatus.Expired)
        {
            return Result.Updated;
        }

        Status = SubscriptionStatus.Active;
        UpdatedAtUtc = activatedAtUtc;
        AddDomainEvent(new Events.SubscriptionActivatedEvent(Id, StudentId, Tier, StartedAtUtc, ExpiresAtUtc));
        return Result.Updated;
    }

    public Result<Updated> StartTrial(DateTimeOffset startedAtUtc, DateTimeOffset trialEndsAtUtc)
    {
        if (trialEndsAtUtc <= startedAtUtc)
        {
            return SubscriptionErrors.ExpirationRequired;
        }

        Status = SubscriptionStatus.Trialing;
        StartedAtUtc = startedAtUtc;
        TrialEndsAtUtc = trialEndsAtUtc;
        ExpiresAtUtc = trialEndsAtUtc;
        UpdatedAtUtc = startedAtUtc;
        AddDomainEvent(new Events.TrialStartedEvent(Id, StudentId, Tier, startedAtUtc, trialEndsAtUtc));
        return Result.Updated;
    }

    public Result<Updated> Renew(DateTimeOffset renewedAtUtc)
    {
        if (Status is SubscriptionStatus.Cancelled or SubscriptionStatus.Expired)
        {
            return Result.Updated;
        }

        ExpiresAtUtc = BillingCycle switch
        {
            BillingCycle.Monthly => ExpiresAtUtc.AddMonths(1),
            BillingCycle.Yearly => ExpiresAtUtc.AddYears(1),
            _ => ExpiresAtUtc
        };

        Status = SubscriptionStatus.Active;
        UpdatedAtUtc = renewedAtUtc;
        AddDomainEvent(new Events.SubscriptionActivatedEvent(Id, StudentId, Tier, StartedAtUtc, ExpiresAtUtc));
        return Result.Updated;
    }

    public Result<Updated> Cancel(DateTimeOffset cancelledAtUtc)
    {
        Status = SubscriptionStatus.Cancelled;
        CancelledAtUtc = cancelledAtUtc;
        UpdatedAtUtc = cancelledAtUtc;
        AddDomainEvent(new Events.SubscriptionCancelledEvent(Id, StudentId, cancelledAtUtc));
        return Result.Updated;
    }

    public Result<Updated> Expire(DateTimeOffset expiredAtUtc)
    {
        if (expiredAtUtc < ExpiresAtUtc)
        {
            return Result.Updated;
        }

        Status = SubscriptionStatus.Expired;
        UpdatedAtUtc = expiredAtUtc;
        AddDomainEvent(new Events.SubscriptionExpiredEvent(Id, StudentId, expiredAtUtc));
        return Result.Updated;
    }

    public Result<Updated> UpgradeTier(SubscriptionTier tier, DateTimeOffset upgradedAtUtc)
    {
        if (!Enum.IsDefined(typeof(SubscriptionTier), tier))
        {
            return SubscriptionErrors.InvalidTier;
        }

        if (tier <= Tier)
        {
            return Result.Updated;
        }

        var oldTier = Tier;
        Tier = tier;
        UpdatedAtUtc = upgradedAtUtc;
        AddDomainEvent(new Events.SubscriptionUpgradedEvent(Id, StudentId, oldTier, tier, upgradedAtUtc));
        return Result.Updated;
    }

    public Result<Updated> EnableAutoRenew()
    {
        AutoRenewEnabled = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> DisableAutoRenew()
    {
        AutoRenewEnabled = false;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        return Result.Updated;
    }
}