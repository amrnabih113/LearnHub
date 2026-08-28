using LearnHub.Application.Features.Quizzes.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Quizzes.Commands.StartQuizAttempt;

public sealed record StartQuizAttemptCommand(Guid QuizId, Guid StudentId)
    : IRequest<Result<QuizAttemptDto>>;
