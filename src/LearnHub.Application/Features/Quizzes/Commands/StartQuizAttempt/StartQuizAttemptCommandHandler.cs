using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Quizzes.Dtos;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Quizzes.Commands.StartQuizAttempt;

public sealed class StartQuizAttemptCommandHandler(
    IAppDbContext context,
    ICourseAccessService accessService)
    : IRequestHandler<StartQuizAttemptCommand, Result<QuizAttemptDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICourseAccessService _accessService = accessService;

    public async Task<Result<QuizAttemptDto>> Handle(
        StartQuizAttemptCommand request,
        CancellationToken cancellationToken)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.Choices)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

        if (quiz is null)
        {
            return QuizErrors.QuestionNotFound;
        }

        if (quiz.Status != QuizStatus.Published)
        {
            return QuizErrors.NotPublished;
        }

        // Evaluate course enrollment/access using ICourseAccessService
        var accessResult = await _accessService.EvaluateAccessAsync(request.StudentId, quiz.CourseId, cancellationToken);
        if (accessResult.IsError || !accessResult.Value.CanWatchLessons)
        {
            return Error.Forbidden("Quiz.AccessDenied", "Enrolled student access required to start assessment.");
        }

        // Previous attempts check
        var previousAttempts = await _context.QuizAttempts
            .AsNoTracking()
            .Where(a => a.QuizId == quiz.Id && a.StudentId == request.StudentId)
            .ToListAsync(cancellationToken);

        if (previousAttempts.Count >= quiz.PassingPolicy.MaxAttempts)
        {
            return QuizErrors.MaxAttemptsExceeded;
        }

        // Prerequisites & Gating validation
        if (quiz.Type == QuizType.Section && quiz.SectionId.HasValue)
        {
            var sections = await _context.Sections
                .AsNoTracking()
                .Where(s => s.CourseId == quiz.CourseId)
                .OrderBy(s => s.Order)
                .ToListAsync(cancellationToken);

            var currentSection = sections.FirstOrDefault(s => s.Id == quiz.SectionId.Value);
            if (currentSection != null)
            {
                var previousSections = sections.Where(s => s.Order < currentSection.Order).ToList();
                var previousSectionIds = previousSections.Select(s => s.Id).ToList();

                var previousQuizzes = await _context.Quizzes
                    .AsNoTracking()
                    .Where(q => q.CourseId == quiz.CourseId && previousSectionIds.Contains(q.SectionId ?? Guid.Empty) && q.Status == QuizStatus.Published)
                    .ToListAsync(cancellationToken);

                foreach (var prevQuiz in previousQuizzes)
                {
                    bool prevPassed = await _context.QuizAttempts
                        .Include(a => a.Grade)
                        .AsNoTracking()
                        .AnyAsync(a => a.QuizId == prevQuiz.Id && a.StudentId == request.StudentId && a.Grade != null && a.Grade.IsPassed, cancellationToken);

                    if (!prevPassed)
                    {
                        return QuizErrors.SectionLocked;
                    }
                }
            }
        }
        else if (quiz.Type == QuizType.Final)
        {
            // Final Exam Prerequisites: all section quizzes must be passed
            var sectionQuizzes = await _context.Quizzes
                .AsNoTracking()
                .Where(q => q.CourseId == quiz.CourseId && q.Type == QuizType.Section && q.Status == QuizStatus.Published)
                .ToListAsync(cancellationToken);

            foreach (var secQuiz in sectionQuizzes)
            {
                bool passed = await _context.QuizAttempts
                    .Include(a => a.Grade)
                    .AsNoTracking()
                    .AnyAsync(a => a.QuizId == secQuiz.Id && a.StudentId == request.StudentId && a.Grade != null && a.Grade.IsPassed, cancellationToken);

                if (!passed)
                {
                    return QuizErrors.PrerequisitesNotCompleted;
                }
            }
        }

        // Start domain attempt
        var startResult = quiz.StartAttempt(Guid.NewGuid(), request.StudentId, previousAttempts.Count);
        if (startResult.IsError)
        {
            return startResult.Errors;
        }

        var attempt = startResult.Value;
        _context.QuizAttempts.Add(attempt);
        await _context.SaveChangesAsync(cancellationToken);

        // Map DTO without exposing correct answer flags
        var attemptQuestions = quiz.Questions
            .OrderBy(q => q.Order)
            .Select(q => new AttemptQuestionDto(
                q.Id,
                q.Prompt,
                q.Type,
                q.Points,
                q.Order,
                q.Choices.Select(c => new AttemptChoiceDto(c.Id, c.Text)).ToList()))
            .ToList();

        var remainingSeconds = attempt.ExpiresAtUtc.HasValue
            ? (int)Math.Max(0, (attempt.ExpiresAtUtc.Value - DateTimeOffset.UtcNow).TotalSeconds)
            : (int?)null;

        return new QuizAttemptDto(
            attempt.Id,
            attempt.QuizId,
            attempt.CourseId,
            attempt.StudentId,
            attempt.AttemptNumber,
            attempt.Status,
            attempt.StartedAtUtc,
            attempt.ExpiresAtUtc,
            attempt.SubmittedAtUtc,
            remainingSeconds,
            attemptQuestions);
    }
}
