using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Subscriptions;

public sealed class TrialOffer : AuditableEntity
{
    public string StudentId { get; private set; } = default!;
    public SubscriptionTier Tier { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? UsedAtUtc { get; private set; }

    public bool IsUsed => UsedAtUtc.HasValue;

    private TrialOffer() { }

    private TrialOffer(Guid id, string studentId, SubscriptionTier tier, DateTimeOffset expiresAtUtc) : base(id)
    {
        StudentId = studentId;
        Tier = tier;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static Result<TrialOffer> Create(Guid id, string studentId, SubscriptionTier tier, DateTimeOffset expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(studentId))
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

        return new TrialOffer(id, studentId.Trim(), tier, expiresAtUtc);
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