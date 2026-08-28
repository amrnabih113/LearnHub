using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Assessments.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearnHub.Application.Features.Assessments.EventHandlers;

public sealed class QuizPassedDomainEventHandler(
    IAppDbContext context,
    ILogger<QuizPassedDomainEventHandler> logger)
    : INotificationHandler<QuizPassedDomainEvent>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<QuizPassedDomainEventHandler> _logger = logger;

    public async Task Handle(QuizPassedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Quiz {QuizId} passed by Student {StudentId} with score {Score}%. Processing progression...",
            notification.QuizId,
            notification.StudentId,
            notification.ScorePercentage);

        var quiz = await _context.Quizzes
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == notification.QuizId, cancellationToken);

        if (quiz is null) return;

        var enrollment = await _context.Enrollments
            .Include(e => e.LessonsProgress)
            .FirstOrDefaultAsync(e => e.CourseId == quiz.CourseId && e.StudentId == notification.StudentId, cancellationToken);

        if (enrollment is null) return;

        if (quiz.Type == QuizType.Final)
        {
            _logger.LogInformation(
                "Final Exam passed by Student {StudentId} for Course {CourseId}. Marking enrollment complete...",
                notification.StudentId,
                quiz.CourseId);

            enrollment.MarkCompleted();
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        if (quiz.Type == QuizType.Section)
        {
            // Check if course has a final exam
            bool hasFinalExam = await _context.Quizzes
                .AsNoTracking()
                .AnyAsync(q => q.CourseId == quiz.CourseId && q.Type == QuizType.Final && q.Status == QuizStatus.Published, cancellationToken);

            if (!hasFinalExam)
            {
                // Check if all lessons and all section quizzes are completed/passed
                var allLessons = await _context.Lessons
                    .AsNoTracking()
                    .Where(l => l.Section.CourseId == quiz.CourseId)
                    .Select(l => l.Id)
                    .ToListAsync(cancellationToken);

                var completedLessonIds = enrollment.LessonsProgress
                    .Where(lp => lp.IsCompleted)
                    .Select(lp => lp.LessonId)
                    .ToHashSet();

                bool allLessonsCompleted = allLessons.All(completedLessonIds.Contains);

                var sectionQuizzes = await _context.Quizzes
                    .AsNoTracking()
                    .Where(q => q.CourseId == quiz.CourseId && q.Type == QuizType.Section && q.Status == QuizStatus.Published)
                    .ToListAsync(cancellationToken);

                var passedQuizIds = await _context.QuizAttempts
                    .Include(a => a.Grade)
                    .AsNoTracking()
                    .Where(a => a.CourseId == quiz.CourseId && a.StudentId == notification.StudentId && a.Grade != null && a.Grade.IsPassed)
                    .Select(a => a.QuizId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                bool allSectionQuizzesPassed = sectionQuizzes.All(sq => passedQuizIds.Contains(sq.Id));

                if (allLessonsCompleted && allSectionQuizzesPassed)
                {
                    _logger.LogInformation(
                        "All course content and section quizzes completed for Course {CourseId}. Marking enrollment complete...",
                        quiz.CourseId);

                    enrollment.MarkCompleted();
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
