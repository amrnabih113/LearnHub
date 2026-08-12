namespace LearnHub.Contracts.Enrollments.Requests;

public sealed record UpdateEnrollmentProgressRequest(
    Guid LessonId,
    int WatchDurationSeconds,
    int TotalLessons,
    int? LessonDurationSeconds = null);
