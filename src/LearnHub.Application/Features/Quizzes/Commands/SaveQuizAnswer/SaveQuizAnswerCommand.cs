using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Quizzes.Commands.SaveQuizAnswer;

public sealed record SaveQuizAnswerCommand(
    Guid AttemptId,
    Guid QuestionId,
    Guid StudentId,
    Guid? SelectedChoiceId = null,
    string? TextAnswer = null)
    : IRequest<Result<Updated>>;
