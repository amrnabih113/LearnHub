namespace LearnHub.Application.Features.Courses.Dtos;

public sealed record AssessmentContentDto(
    Guid Id,
    string Type,
    string Title,
    string? Description,
    bool IsRequired,
    int QuestionCount,
    int TotalPoints,
    int? TimeLimitMinutes,
    int PassingPercentage,
    int AttemptsAllowed,
    int AttemptsUsed,
    int AttemptsRemaining,
    decimal? BestScore,
    decimal? LatestScore,
    string Status,
    bool IsLocked,
    bool CanStart);
