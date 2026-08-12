using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses;
using LearnHub.Domain.Enrollments.Certificates;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Enrollments.Events;
using LearnHub.Domain.Identity;
using LessonProgressEntity = LearnHub.Domain.Enrollments.LessonProgress.LessonProgress;

namespace LearnHub.Domain.Enrollments;


public sealed class Enrollment : AuditableEntity
{
    public Guid StudentId { get; private set; }

    public User Student { get; private set; } = default!;


    public Guid CourseId { get; private set; }

    public Course Course { get; private set; } = default!;

    public EnrollmentStatus Status { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public decimal ProgressPercentage { get; private set; }

    public Certificate? Certificate { get; private set; }

    private readonly List<LessonProgressEntity> _lessonsProgress = [];

    public IReadOnlyCollection<LessonProgressEntity> LessonsProgress => _lessonsProgress.AsReadOnly();

    private Enrollment() { }

    private Enrollment(Guid id, Guid studentId, Guid courseId) : base(id)
    {
        StudentId = studentId;
        CourseId = courseId;
        Status = EnrollmentStatus.Active;
        ProgressPercentage = 0;
        AddDomainEvent(new EnrollmentCreatedDomainEvent(id, studentId, courseId));
    }

    public static Result<Enrollment> Create(Guid id, Guid studentId, Guid courseId)
    {
        if (studentId == Guid.Empty)
        {
            return EnrollmentErrors.StudentIdRequired;
        }

        if (courseId == Guid.Empty)
        {
            return EnrollmentErrors.CourseIdRequired;
        }

        return new Enrollment(id, studentId, courseId);
    }

    public Result<Updated> UpdateWatchProgress(Guid lessonId, int watchDurationSeconds, int totalLessons, int? lessonDurationSeconds = null)
    {
        if (Status != EnrollmentStatus.Active)
        {
            return EnrollmentErrors.NotActive;
        }

        if (totalLessons <= 0)
        {
            return EnrollmentErrors.TotalLessonsInvalid;
        }

        if (lessonId == Guid.Empty)
        {
            return EnrollmentErrors.LessonIdRequired;
        }

        var lessonProgress = _lessonsProgress.FirstOrDefault(lp => lp.LessonId == lessonId);
        if (lessonProgress is null)
        {
            var createResult = LessonProgressEntity.Create(Guid.NewGuid(), Id, lessonId);
            if (createResult.IsError)
            {
                return createResult.Errors;
            }

            lessonProgress = createResult.Value;
            _lessonsProgress.Add(lessonProgress);
        }

        var updateResult = lessonProgress.UpdateWatchProgress(watchDurationSeconds, lessonDurationSeconds);
        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        return UpdateCourseProgress(totalLessons);
    }

    public Result<Updated> CompleteLesson(Guid lessonId, int totalLessons)
    {
        if (Status != EnrollmentStatus.Active)
        {
            return EnrollmentErrors.NotActive;
        }

        if (totalLessons <= 0)
        {
            return EnrollmentErrors.TotalLessonsInvalid;
        }

        if (lessonId == Guid.Empty)
        {
            return EnrollmentErrors.LessonIdRequired;
        }

        var lessonProgress = _lessonsProgress.FirstOrDefault(lp => lp.LessonId == lessonId);
        if (lessonProgress is null)
        {
            var createResult = LessonProgressEntity.Create(Guid.NewGuid(), Id, lessonId);
            if (createResult.IsError)
            {
                return createResult.Errors;
            }

            lessonProgress = createResult.Value;
            _lessonsProgress.Add(lessonProgress);
        }

        var markResult = lessonProgress.MarkCompleted();
        if (markResult.IsError)
        {
            return markResult.Errors;
        }

        return UpdateCourseProgress(totalLessons);
    }

    public Result<Updated> MarkCompleted()
    {
        if (Status == EnrollmentStatus.Dropped)
        {
            return EnrollmentErrors.Dropped;
        }

        if (Status == EnrollmentStatus.Completed)
        {
            return Result.Updated;
        }

        Status = EnrollmentStatus.Completed;
        CompletedAtUtc = DateTimeOffset.UtcNow;
        ProgressPercentage = 100;
        EnsureCertificate();
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        AddDomainEvent(new CourseCompletedDomainEvent(Id, CourseId, StudentId));

        return Result.Updated;
    }

    public Result<Updated> Cancel()
    {
        if (Status == EnrollmentStatus.Completed)
        {
            return EnrollmentErrors.AlreadyCompleted;
        }

        if (Status == EnrollmentStatus.Dropped)
        {
            return Result.Updated;
        }

        Status = EnrollmentStatus.Dropped;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    private Result<Updated> UpdateCourseProgress(int totalLessons)
    {
        var completedLessonsCount = _lessonsProgress.Count(lp => lp.IsCompleted);
        var progress = Math.Min(100m, decimal.Round((completedLessonsCount * 100m) / totalLessons, 2));

        ProgressPercentage = progress;
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        if (ProgressPercentage >= 100m)
        {
            return MarkCompleted();
        }

        return Result.Updated;
    }

    private void EnsureCertificate()
    {
        if (Certificate is not null)
        {
            return;
        }
        var code = $"CERT-{CourseId:N}-{Id:N}";

        code = code[..Math.Min(code.Length, 32)];
        var certificateResult = Certificate.Create(
            Guid.NewGuid(),
            Id,
            StudentId,
           code);

        if (certificateResult.IsSuccess)
        {
            Certificate = certificateResult.Value;
        }
    }
}

