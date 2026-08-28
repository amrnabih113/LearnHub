using LearnHub.Domain.Assessments.Enums;

namespace LearnHub.Application.Features.Quizzes.Dtos;

public sealed record QuizDto(
    Guid Id,
    Guid CourseId,
    Guid? SectionId,
    QuizType Type,
    string Title,
    string? Description,
    QuizStatus Status,
    int? TimeLimitMinutes,
    int MaxAttempts,
    int PassPercentage,
    int QuestionCount,
    int TotalPoints,
    IReadOnlyList<QuestionDto> Questions);

public sealed record QuestionDto(
    Guid Id,
    string Prompt,
    QuestionType Type,
    int Points,
    int Order,
    IReadOnlyList<ChoiceDto> Choices);

public sealed record ChoiceDto(
    Guid Id,
    string Text,
    bool? IsCorrect = null);

public sealed record QuizAttemptDto(
    Guid AttemptId,
    Guid QuizId,
    Guid CourseId,
    Guid StudentId,
    int AttemptNumber,
    QuizAttemptStatus Status,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset? ExpiresAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    int? RemainingTimeSeconds,
    IReadOnlyList<AttemptQuestionDto> Questions);

public sealed record AttemptQuestionDto(
    Guid QuestionId,
    string Prompt,
    QuestionType Type,
    int Points,
    int Order,
    IReadOnlyList<AttemptChoiceDto> Choices,
    Guid? SelectedChoiceId = null,
    string? TextAnswer = null);

public sealed record AttemptChoiceDto(
    Guid ChoiceId,
    string Text);

public sealed record QuizAttemptResultDto(
    Guid AttemptId,
    Guid QuizId,
    Guid StudentId,
    int AttemptNumber,
    QuizAttemptStatus Status,
    decimal ScorePercentage,
    bool IsPassed,
    int PassPercentage,
    DateTimeOffset SubmittedAtUtc,
    int AttemptsRemaining);
