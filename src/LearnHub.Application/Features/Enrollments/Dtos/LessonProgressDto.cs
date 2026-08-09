namespace LearnHub.Application.Features.Enrollments.Dtos;

public sealed record LessonProgressDto(
    Guid Id,
    Guid LessonId,
    int WatchDurationSeconds,
    bool IsCompleted,
    DateTimeOffset? CompletedAtUtc,
    DateTimeOffset CreatedAtUtc);
