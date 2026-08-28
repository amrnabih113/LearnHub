using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Quizzes.Dtos;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Quizzes.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandHandler(IAppDbContext context)
    : IRequestHandler<SubmitQuizAttemptCommand, Result<QuizAttemptResultDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<QuizAttemptResultDto>> Handle(
        SubmitQuizAttemptCommand request,
        CancellationToken cancellationToken)
    {
        var attempt = await _context.QuizAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == request.AttemptId, cancellationToken);

        if (attempt is null)
        {
            return QuizAttemptErrors.AttemptNotFound;
        }

        if (attempt.StudentId != request.StudentId)
        {
            return QuizAttemptErrors.StudentMismatch;
        }

        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Choices)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == attempt.QuizId, cancellationToken);

        if (quiz is null)
        {
            return QuizErrors.QuestionNotFound;
        }

        var submitResult = attempt.Submit(quiz, DateTimeOffset.UtcNow);
        if (submitResult.IsError)
        {
            return submitResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        int attemptsCount = await _context.QuizAttempts
            .AsNoTracking()
            .CountAsync(a => a.QuizId == quiz.Id && a.StudentId == request.StudentId, cancellationToken);

        int attemptsRemaining = Math.Max(0, quiz.PassingPolicy.MaxAttempts - attemptsCount);

        return new QuizAttemptResultDto(
            AttemptId: attempt.Id,
            QuizId: attempt.QuizId,
            StudentId: attempt.StudentId,
            AttemptNumber: attempt.AttemptNumber,
            Status: attempt.Status,
            ScorePercentage: attempt.ScorePercentage ?? 0m,
            IsPassed: attempt.Grade?.IsPassed ?? false,
            PassPercentage: quiz.PassingPolicy.PassPercentage,
            SubmittedAtUtc: attempt.SubmittedAtUtc ?? DateTimeOffset.UtcNow,
            AttemptsRemaining: attemptsRemaining);
    }
}
