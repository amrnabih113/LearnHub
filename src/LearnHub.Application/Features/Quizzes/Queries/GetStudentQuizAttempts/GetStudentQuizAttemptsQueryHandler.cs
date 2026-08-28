using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Quizzes.Dtos;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Quizzes.Queries.GetStudentQuizAttempts;

public sealed record GetStudentQuizAttemptsQuery(Guid QuizId, Guid StudentId)
    : IRequest<Result<IReadOnlyList<QuizAttemptResultDto>>>;

public sealed class GetStudentQuizAttemptsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetStudentQuizAttemptsQuery, Result<IReadOnlyList<QuizAttemptResultDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<QuizAttemptResultDto>>> Handle(
        GetStudentQuizAttemptsQuery request,
        CancellationToken cancellationToken)
    {
        var quiz = await _context.Quizzes
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

        if (quiz is null)
        {
            return QuizErrors.QuestionNotFound;
        }

        var attempts = await _context.QuizAttempts
            .Include(a => a.Grade)
            .AsNoTracking()
            .Where(a => a.QuizId == request.QuizId && a.StudentId == request.StudentId)
            .OrderByDescending(a => a.AttemptNumber)
            .ToListAsync(cancellationToken);

        int totalAttemptsCount = attempts.Count;

        var dtos = attempts.Select(a => new QuizAttemptResultDto(
            AttemptId: a.Id,
            QuizId: a.QuizId,
            StudentId: a.StudentId,
            AttemptNumber: a.AttemptNumber,
            Status: a.Status,
            ScorePercentage: a.ScorePercentage ?? 0m,
            IsPassed: a.Grade?.IsPassed ?? false,
            PassPercentage: quiz.PassingPolicy.PassPercentage,
            SubmittedAtUtc: a.SubmittedAtUtc ?? a.CreatedAtUtc,
            AttemptsRemaining: Math.Max(0, quiz.PassingPolicy.MaxAttempts - totalAttemptsCount)
        )).ToList();

        return dtos;
    }
}
