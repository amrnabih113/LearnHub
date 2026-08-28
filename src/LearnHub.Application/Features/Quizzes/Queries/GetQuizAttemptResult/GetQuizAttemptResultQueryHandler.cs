using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Quizzes.Dtos;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Quizzes.Queries.GetQuizAttemptResult;

public sealed record GetQuizAttemptResultQuery(Guid AttemptId, Guid StudentId)
    : IRequest<Result<QuizAttemptResultDto>>;

public sealed class GetQuizAttemptResultQueryHandler(IAppDbContext context)
    : IRequestHandler<GetQuizAttemptResultQuery, Result<QuizAttemptResultDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<QuizAttemptResultDto>> Handle(
        GetQuizAttemptResultQuery request,
        CancellationToken cancellationToken)
    {
        var attempt = await _context.QuizAttempts
            .Include(a => a.Grade)
            .AsNoTracking()
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
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == attempt.QuizId, cancellationToken);

        if (quiz is null)
        {
            return QuizErrors.QuestionNotFound;
        }

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
            SubmittedAtUtc: attempt.SubmittedAtUtc ?? attempt.CreatedAtUtc,
            AttemptsRemaining: attemptsRemaining);
    }
}
