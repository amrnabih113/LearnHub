using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Subscriptions;

public sealed class TrialOffer : AuditableEntity
{
    public Guid StudentId { get; private set; }
    public SubscriptionTier Tier { get; private set; }
    public int DurationDays { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? UsedAtUtc { get; private set; }

    public bool IsUsed => UsedAtUtc.HasValue;

    private TrialOffer() { }

    private TrialOffer(Guid id, Guid studentId, SubscriptionTier tier, int durationDays, DateTimeOffset expiresAtUtc) : base(id)
    {
        StudentId = studentId;
        Tier = tier;
        DurationDays = durationDays;
        ExpiresAtUtc = expiresAtUtc;
    }
    public static Result<TrialOffer> Create(Guid id, Guid studentId, SubscriptionTier tier, int durationDays, DateTimeOffset expiresAtUtc)
    {
        if (studentId == Guid.Empty)
        {
            return SubscriptionErrors.StudentIdRequired;
        }

        if (!Enum.IsDefined(typeof(SubscriptionTier), tier))
        {
            return SubscriptionErrors.InvalidTier;
        }

        if (expiresAtUtc <= DateTimeOffset.UtcNow)
        {
            return SubscriptionErrors.TrialExpired;
        }

        return new TrialOffer(id, studentId, tier, durationDays, expiresAtUtc);
    }

    public bool IsActive(DateTimeOffset now) => !IsUsed && now < ExpiresAtUtc;

    public Result<Updated> Use(DateTimeOffset usedAtUtc)
    {
        if (IsUsed)
        {
            return SubscriptionErrors.TrialAlreadyUsed;
        }

        if (usedAtUtc >= ExpiresAtUtc)
        {
            return SubscriptionErrors.TrialExpired;
        }

        UsedAtUtc = usedAtUtc;
        UpdatedAtUtc = usedAtUtc;
        return Result.Updated;
    }
}