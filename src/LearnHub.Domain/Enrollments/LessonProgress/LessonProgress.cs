using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Enrollments.LessonProgress;

public sealed class LessonProgress : AuditableEntity
{
    public Guid EnrollmentId { get; private set; }

    public Guid LessonId { get; private set; }

    public bool IsCompleted { get; private set; }

    public int WatchDurationSeconds { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    private LessonProgress() { }

    private LessonProgress(Guid id, Guid enrollmentId, Guid lessonId) : base(id)
    {
        EnrollmentId = enrollmentId;
        LessonId = lessonId;
        IsCompleted = false;
        WatchDurationSeconds = 0;
    }
    public static Result<LessonProgress> Create(Guid id, Guid enrollmentId, Guid lessonId)
    {
        if (enrollmentId == Guid.Empty)
        {
            return LessonProgressErrors.EnrollmentIdRequired;
        }

        if (lessonId == Guid.Empty)
        {
            return LessonProgressErrors.LessonIdRequired;
        }

        return new LessonProgress(id, enrollmentId, lessonId);
    }

    public Result<Updated> UpdateWatchProgress(int watchDurationSeconds, int? lessonDurationSeconds = null)
    {
        if (watchDurationSeconds < 0)
        {
            return LessonProgressErrors.InvalidWatchDuration;
        }

        WatchDurationSeconds = Math.Max(WatchDurationSeconds, watchDurationSeconds);
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        if (lessonDurationSeconds is > 0 && WatchDurationSeconds >= lessonDurationSeconds.Value)
        {
            return MarkCompleted();
        }

        return Result.Updated;
    }

    public Result<Updated> MarkCompleted()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletedAtUtc = DateTimeOffset.UtcNow;
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        return Result.Updated;
    }
}