using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetCourseByIdAdmin;

public sealed class GetCourseByIdAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCourseByIdAdminQuery, Result<CourseAdminDetailDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CourseAdminDetailDto>> Handle(
        GetCourseByIdAdminQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
        {
            return Error.NotFound("Course.NotFound", "Course not found.");
        }

        var sectionsList = course.Sections.ToList();
        var sectionsCount = sectionsList.Count;
        var lessonsCount = sectionsList.Sum(s => s.LessonCount);

        var enrollmentsCount = await _context.Enrollments
            .AsNoTracking()
            .CountAsync(e => e.CourseId == request.Id, cancellationToken);

        var reviews = await _context.CourseReviews
            .AsNoTracking()
            .Where(r => r.CourseId == request.Id)
            .Select(r => r.Rating.Value)
            .ToListAsync(cancellationToken);

        var totalReviews = reviews.Count;
        var averageRating = totalReviews > 0 ? Math.Round(reviews.Average(), 1) : 0.0;

        return new CourseAdminDetailDto(
            course.Id,
            course.Title,
            course.Description,
            course.Status.ToString(),
            course.Price.Amount,
            course.Price.Currency,
            course.InstructorId,
            course.Instructor?.FullName ?? string.Empty,
            course.Instructor?.Email ?? string.Empty,
            course.CategoryId,
            course.Category?.Name ?? string.Empty,
            course.Level.ToString(),
            course.IsIncludedInSubscription,
            course.RequiredSubscriptionTier.ToString(),
            sectionsCount,
            lessonsCount,
            enrollmentsCount,
            averageRating,
            totalReviews,
            course.CreatedAtUtc);
    }
}
