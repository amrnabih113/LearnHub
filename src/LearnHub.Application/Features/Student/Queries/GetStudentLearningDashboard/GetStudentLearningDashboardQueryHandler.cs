using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Student.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Student.Queries.GetStudentLearningDashboard;

public sealed class GetStudentLearningDashboardQueryHandler(
    IAppDbContext context,
    ICourseAccessService accessService)
    : IRequestHandler<GetStudentLearningDashboardQuery, Result<StudentLearningDashboardDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICourseAccessService _accessService = accessService;

    public async Task<Result<StudentLearningDashboardDto>> Handle(
        GetStudentLearningDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
                .ThenInclude(c => c.Category)
            .Include(e => e.Course)
                .ThenInclude(c => c.Instructor)
            .Include(e => e.Course)
                .ThenInclude(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
            .AsNoTracking()
            .Where(e => e.StudentId == request.StudentId)
            .OrderByDescending(e => e.UpdatedAtUtc)
            .ToListAsync(cancellationToken);

        var courseItems = new List<EnrolledCourseProgressDto>();
        int activeCount = 0;
        int completedCount = 0;
        int pausedCount = 0;

        foreach (var enrollment in enrollments)
        {
            var course = enrollment.Course;
            if (course is null) continue;

            if (enrollment.Status == EnrollmentStatus.Active) activeCount++;
            else if (enrollment.Status == EnrollmentStatus.Completed) completedCount++;
            else if (enrollment.Status == EnrollmentStatus.Dropped) pausedCount++;

            // Evaluate access using ICourseAccessService (single source of truth)
            var accessResult = await _accessService.EvaluateAccessAsync(request.StudentId, course.Id, cancellationToken);
            bool isAccessible = accessResult.IsSuccess && accessResult.Value.IsAccessible;
            bool canWatchLessons = accessResult.IsSuccess && accessResult.Value.CanWatchLessons;

            // Find next uncompleted lesson
            Guid? nextLessonId = null;
            string? nextLessonTitle = null;

            var allLessons = course.Sections
                .OrderBy(s => s.Order)
                .SelectMany(s => s.Lessons.OrderBy(l => l.Order))
                .ToList();

            if (allLessons.Count > 0)
            {
                var completedLessonIds = await _context.LessonProgresses
                    .AsNoTracking()
                    .Where(lp => lp.EnrollmentId == enrollment.Id && lp.IsCompleted)
                    .Select(lp => lp.LessonId)
                    .ToListAsync(cancellationToken);

                var nextLesson = allLessons.FirstOrDefault(l => !completedLessonIds.Contains(l.Id)) ?? allLessons.First();
                nextLessonId = nextLesson.Id;
                nextLessonTitle = nextLesson.Title;
            }

            var instructorName = course.Instructor != null
                ? $"{course.Instructor.FirstName} {course.Instructor.LastName}"
                : "LearnHub Instructor";

            courseItems.Add(new EnrolledCourseProgressDto(
                EnrollmentId: enrollment.Id,
                CourseId: course.Id,
                CourseTitle: course.Title,
                ThumbnailUrl: course.ThumbnailUrl,
                CategoryName: course.Category != null ? course.Category.Name : string.Empty,
                InstructorName: instructorName,
                ProgressPercentage: enrollment.ProgressPercentage,
                Status: enrollment.Status,
                CompletedAtUtc: enrollment.CompletedAtUtc,
                LastAccessedAtUtc: enrollment.UpdatedAtUtc ?? enrollment.CreatedAtUtc,
                IsAccessible: isAccessible,
                CanWatchLessons: canWatchLessons,
                NextLessonId: nextLessonId,
                NextLessonTitle: nextLessonTitle));
        }

        return new StudentLearningDashboardDto(
            Courses: courseItems,
            TotalEnrolled: enrollments.Count,
            ActiveCount: activeCount,
            CompletedCount: completedCount,
            PausedCount: pausedCount);
    }
}
