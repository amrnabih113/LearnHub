using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.LearningPaths.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.LearningPaths;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.LearningPaths.Queries.GetLearningPathById;

public sealed class GetLearningPathByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetLearningPathByIdQuery, Result<LearningPathDetailDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<LearningPathDetailDto>> Handle(
        GetLearningPathByIdQuery request,
        CancellationToken cancellationToken)
    {
        var path = await _context.LearningPaths
            .Include(lp => lp.Owner)
            .Include(lp => lp.Courses)
                .ThenInclude(lpc => lpc.Course)
                    .ThenInclude(c => c.Category)
            .Include(lp => lp.Courses)
                .ThenInclude(lpc => lpc.Course)
                    .ThenInclude(c => c.Instructor)
            .AsNoTracking()
            .FirstOrDefaultAsync(lp => lp.Id == request.LearningPathId, cancellationToken);

        if (path is null)
        {
            return LearningPathErrors.NotFound;
        }

        var orderedCourses = path.Courses
            .OrderBy(lpc => lpc.Order)
            .Select(lpc =>
            {
                var c = lpc.Course;
                var avgRating = _context.CourseReviews
                    .Where(r => r.CourseId == c.Id)
                    .Select(r => (double?)r.Rating.Value)
                    .Average() ?? 0.0;

                var enrollmentCount = _context.Enrollments
                    .Count(e => e.CourseId == c.Id);

                return new LearningPathCourseDto(
                    CourseId: c.Id,
                    Title: c.Title,
                    ThumbnailUrl: c.ThumbnailUrl,
                    CategoryName: c.Category != null ? c.Category.Name : string.Empty,
                    InstructorName: c.Instructor != null ? (c.Instructor.FirstName + " " + c.Instructor.LastName) : "LearnHub Instructor",
                    Level: c.Level,
                    Price: c.Price.Amount,
                    Currency: c.Price.Currency,
                    IsFree: c.Price.Amount == 0,
                    IsIncludedInSubscription: c.IsIncludedInSubscription,
                    Order: lpc.Order,
                    IsRequired: lpc.IsRequired,
                    AverageRating: Math.Round(avgRating, 1),
                    EnrollmentCount: enrollmentCount);
            })
            .ToList();

        return new LearningPathDetailDto(
            path.Id,
            path.Title,
            path.Slug,
            path.Description,
            path.ShortDescription,
            path.ThumbnailUrl,
            path.Level,
            path.Status,
            path.OwnerId,
            path.Owner != null ? path.Owner.FullName : null,
            orderedCourses.Count,
            orderedCourses,
            path.CreatedAtUtc,
            path.PublishedAtUtc);
    }
}
