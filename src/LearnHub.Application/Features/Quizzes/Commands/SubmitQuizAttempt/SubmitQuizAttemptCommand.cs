using LearnHub.Application.Features.Quizzes.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;

namespace LearnHub.Application.Features.Quizzes.Commands.SubmitQuizAttempt;

public sealed record SubmitQuizAttemptCommand(Guid AttemptId, Guid StudentId)
    : IRequest<Result<QuizAttemptResultDto>>;
