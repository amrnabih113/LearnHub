using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Purchasing.Subscriptions;

public sealed class Subscription : AuditableEntity
{
    public string StudentId { get; private set; } = default!;
    public Guid CourseId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTimeOffset StartedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? PausedAtUtc { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }

    private Subscription() { }

    private Subscription(Guid id, string studentId, Guid courseId, DateTimeOffset startedAtUtc, DateTimeOffset expiresAtUtc) : base(id)
    {
        StudentId = studentId;
        CourseId = courseId;
        StartedAtUtc = startedAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        Status = SubscriptionStatus.Active;
    }

    public static Result<Subscription> Create(Guid id, string studentId, Guid courseId, DateTimeOffset startedAtUtc, DateTimeOffset expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(studentId))
        {
            return SubscriptionErrors.StudentIdRequired;
        }

        if (courseId == Guid.Empty)
        {
            return SubscriptionErrors.CourseIdRequired;
        }

        if (expiresAtUtc <= startedAtUtc)
        {
            return SubscriptionErrors.ExpirationRequired;
        }

        return new Subscription(id, studentId.Trim(), courseId, startedAtUtc, expiresAtUtc);
    }

    public Result<Updated> Pause(DateTimeOffset pausedAtUtc)
    {
        if (Status != SubscriptionStatus.Active)
        {
            return Result.Updated;
        }

        Status = SubscriptionStatus.Paused;
        PausedAtUtc = pausedAtUtc;
        UpdatedAtUtc = pausedAtUtc;
        return Result.Updated;
    }

    public Result<Updated> Resume(DateTimeOffset resumedAtUtc)
    {
        if (Status != SubscriptionStatus.Paused)
        {
            return Result.Updated;
        }

        Status = SubscriptionStatus.Active;
        UpdatedAtUtc = resumedAtUtc;
        return Result.Updated;
    }

    public Result<Updated> Cancel(DateTimeOffset cancelledAtUtc)
    {
        Status = SubscriptionStatus.Cancelled;
        CancelledAtUtc = cancelledAtUtc;
        UpdatedAtUtc = cancelledAtUtc;
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
        return Result.Updated;
    }
}
