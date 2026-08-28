using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Features.Courses.Dtos;
using LearnHub.Domain.Assessments;
using LearnHub.Domain.Assessments.Enums;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Courses.Queries.GetCourseContent;

public sealed class GetCourseContentQueryHandler(
    IAppDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetCourseContentQuery, Result<CourseContentDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<Result<CourseContentDto>> Handle(
        GetCourseContentQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Include(x => x.Sections)
                .ThenInclude(x => x.Lessons)
                    .ThenInclude(x => x.Resources)
            .FirstOrDefaultAsync(x => x.Id == request.CourseId, cancellationToken);

        if (course is null)
        {
            return Error.NotFound("ApplicationError.Course.NotFound", "Course not found.");
        }

        var studentId = _currentUserService.UserId;

        // Fetch all published quizzes for this course
        var quizzes = await _context.Quizzes
            .Include(q => q.Questions)
            .AsNoTracking()
            .Where(q => q.CourseId == request.CourseId && q.Status == QuizStatus.Published)
            .ToListAsync(cancellationToken);

        // Fetch student's attempts for these quizzes
        var quizIds = quizzes.Select(q => q.Id).ToList();
        var attempts = studentId.HasValue && quizIds.Count > 0
            ? await _context.QuizAttempts
                .Include(a => a.Grade)
                .AsNoTracking()
                .Where(a => quizIds.Contains(a.QuizId) && a.StudentId == studentId.Value)
                .ToListAsync(cancellationToken)
            : [];

        // Fetch student's completed lessons if enrolled
        var completedLessonIds = new HashSet<Guid>();
        if (studentId.HasValue)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.LessonsProgress)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.CourseId == request.CourseId && e.StudentId == studentId.Value, cancellationToken);

            if (enrollment != null)
            {
                completedLessonIds = enrollment.LessonsProgress
                    .Where(lp => lp.IsCompleted)
                    .Select(lp => lp.LessonId)
                    .ToHashSet();
            }
        }

        var sortedSections = course.Sections.OrderBy(s => s.Order).ToList();
        var sectionDtos = new List<SectionDto>();
        bool previousSectionPassed = true; // Section 1 is unlocked by default

        foreach (var section in sortedSections)
        {
            bool isSectionLocked = !previousSectionPassed;

            // Map lessons
            var lessonDtos = section.Lessons
                .OrderBy(l => l.Order)
                .Select(l => new LessonDto(
                    l.Id,
                    l.Title ?? string.Empty,
                    l.Description ?? string.Empty,
                    l.VideoUrl ?? string.Empty,
                    l.IsPreview,
                    l.Content ?? string.Empty,
                    l.DurationInMinutes,
                    l.Order,
                    l.Resources.Select(r => new ResourceDto(r.Id, r.Name, r.Url, r.Type, r.SizeInBytes)).ToList()))
                .ToList();

            // Find Section Quiz
            var sectionQuiz = quizzes.FirstOrDefault(q => q.SectionId == section.Id && q.Type == QuizType.Section);
            AssessmentContentDto? assessmentDto = null;

            if (sectionQuiz != null)
            {
                var quizAttempts = attempts.Where(a => a.QuizId == sectionQuiz.Id).ToList();
                assessmentDto = MapAssessmentDto(sectionQuiz, quizAttempts, isSectionLocked);

                // Section N+1 is unlocked only if Section N's quiz is passed (if present)
                previousSectionPassed = !isSectionLocked && (assessmentDto.Status == "Passed");
            }
            else
            {
                // If no section quiz, check if section lessons are completed
                var sectionLessonIds = section.Lessons.Select(l => l.Id).ToList();
                bool allSectionLessonsCompleted = sectionLessonIds.Count == 0 || sectionLessonIds.All(completedLessonIds.Contains);
                previousSectionPassed = !isSectionLocked && allSectionLessonsCompleted;
            }

            sectionDtos.Add(new SectionDto(
                section.Id,
                section.Title,
                section.Description,
                section.Order,
                section.LessonCount,
                section.DurationInMinutes,
                lessonDtos,
                assessmentDto,
                isSectionLocked));
        }

        // Map Final Exam
        var finalExam = quizzes.FirstOrDefault(q => q.Type == QuizType.Final);
        AssessmentContentDto? finalAssessmentDto = null;

        if (finalExam != null)
        {
            // Final Exam is unlocked only if all sections were passed / completed
            bool isFinalExamLocked = !previousSectionPassed;
            var finalAttempts = attempts.Where(a => a.QuizId == finalExam.Id).ToList();
            finalAssessmentDto = MapAssessmentDto(finalExam, finalAttempts, isFinalExamLocked);
        }

        return new CourseContentDto(
            course.Id,
            sectionDtos,
            finalAssessmentDto);
    }

    private static AssessmentContentDto MapAssessmentDto(
        Quiz quiz,
        List<QuizAttempt> attempts,
        bool isParentLocked)
    {
        int attemptsAllowed = quiz.PassingPolicy.MaxAttempts;
        int attemptsUsed = attempts.Count;
        int attemptsRemaining = Math.Max(0, attemptsAllowed - attemptsUsed);

        var submittedAttempts = attempts.Where(a => a.Status == QuizAttemptStatus.Submitted && a.ScorePercentage.HasValue).ToList();
        decimal? bestScore = submittedAttempts.Count > 0 ? submittedAttempts.Max(a => a.ScorePercentage!.Value) : null;
        decimal? latestScore = submittedAttempts.Count > 0 ? submittedAttempts.OrderByDescending(a => a.SubmittedAtUtc).First().ScorePercentage!.Value : null;

        bool hasPassed = submittedAttempts.Any(a => a.Grade != null && a.Grade.IsPassed);
        bool hasInProgress = attempts.Any(a => a.Status == QuizAttemptStatus.InProgress);

        string status;
        if (isParentLocked)
        {
            status = "Locked";
        }
        else if (hasPassed)
        {
            status = "Passed";
        }
        else if (hasInProgress)
        {
            status = "InProgress";
        }
        else if (attemptsRemaining <= 0)
        {
            status = "AttemptsExhausted";
        }
        else if (attemptsUsed > 0 && !hasPassed)
        {
            status = "Failed";
        }
        else
        {
            status = "Available";
        }

        bool isLocked = isParentLocked || status == "Locked";
        bool canStart = !isLocked && !hasPassed && attemptsRemaining > 0 && !hasInProgress;

        return new AssessmentContentDto(
            Id: quiz.Id,
            Type: quiz.Type.ToString(),
            Title: quiz.Title,
            Description: quiz.Description,
            IsRequired: true,
            QuestionCount: quiz.Questions.Count,
            TotalPoints: quiz.TotalPoints(),
            TimeLimitMinutes: quiz.TimeLimitMinutes,
            PassingPercentage: quiz.PassingPolicy.PassPercentage,
            AttemptsAllowed: attemptsAllowed,
            AttemptsUsed: attemptsUsed,
            AttemptsRemaining: attemptsRemaining,
            BestScore: bestScore,
            LatestScore: latestScore,
            Status: status,
            IsLocked: isLocked,
            CanStart: canStart);
    }
}