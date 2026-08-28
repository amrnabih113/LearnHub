using LearnHub.Domain.Assessments.Enums;

namespace LearnHub.Contracts.Quizzes.Requests;

public sealed record CreateSectionQuizRequest(
    Guid CourseId,
    string Title,
    string? Description,
    int? TimeLimitMinutes,
    int MaxAttempts = 3,
    int PassPercentage = 70);

public sealed record CreateFinalExamRequest(
    string Title,
    string? Description,
    int? TimeLimitMinutes,
    int MaxAttempts = 3,
    int PassPercentage = 70);

public sealed record ChoiceRequest(string Text, bool IsCorrect);

public sealed record AddQuestionRequest(
    string Prompt,
    QuestionType Type,
    int Points,
    int Order,
    IReadOnlyList<ChoiceRequest>? Choices = null,
    string? CorrectTextAnswer = null);
