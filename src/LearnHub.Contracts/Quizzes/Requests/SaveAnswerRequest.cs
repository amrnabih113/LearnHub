namespace LearnHub.Contracts.Quizzes.Requests;

public sealed record SaveAnswerRequest(
    Guid? SelectedChoiceId = null,
    string? TextAnswer = null);
