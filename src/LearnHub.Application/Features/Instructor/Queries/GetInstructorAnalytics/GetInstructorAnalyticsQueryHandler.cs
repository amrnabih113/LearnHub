using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Instructor.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Purchasing.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Instructor.Queries.GetInstructorAnalytics;

public sealed class GetInstructorAnalyticsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetInstructorAnalyticsQuery, Result<IReadOnlyList<InstructorCourseAnalyticsDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<InstructorCourseAnalyticsDto>>> Handle(
        GetInstructorAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        var courseQuery = _context.Courses
            .AsNoTracking()
            .Where(c => c.InstructorId == request.InstructorId);

        if (request.CourseId.HasValue)
        {
            courseQuery = courseQuery.Where(c => c.Id == request.CourseId.Value);
        }

        var courses = await courseQuery.ToListAsync(cancellationToken);
        var courseIds = courses.Select(c => c.Id).ToList();

        if (courseIds.Count == 0)
        {
            return new List<InstructorCourseAnalyticsDto>();
        }

        var paidOrders = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Paid && o.Items.Any(i => courseIds.Contains(i.CourseId)))
            .ToListAsync(cancellationToken);

        var enrollments = await _context.Enrollments
            .AsNoTracking()
            .Where(e => courseIds.Contains(e.CourseId))
            .ToListAsync(cancellationToken);

        var reviews = await _context.CourseReviews
            .AsNoTracking()
            .Where(r => courseIds.Contains(r.CourseId) && r.Status == Domain.Reviews.Enums.ReviewStatus.Published)
            .ToListAsync(cancellationToken);

        var analyticsList = new List<InstructorCourseAnalyticsDto>();

        foreach (var course in courses)
        {
            var courseEnrollments = enrollments.Where(e => e.CourseId == course.Id).ToList();
            int totalStudents = courseEnrollments.Count;
            int activeStudents = courseEnrollments.Count(e => e.Status == EnrollmentStatus.Active);
            int completedStudents = courseEnrollments.Count(e => e.Status == EnrollmentStatus.Completed);

            decimal avgProgress = totalStudents > 0
                ? Math.Round(courseEnrollments.Average(e => e.ProgressPercentage), 2)
                : 0m;

            decimal courseRevenue = paidOrders
                .SelectMany(o => o.Items)
                .Where(i => i.CourseId == course.Id)
                .Sum(i => i.UnitPriceSnapshot.Amount);

            var courseRatings = reviews.Where(r => r.CourseId == course.Id).Select(r => r.Rating.Value).ToList();
            double avgRating = courseRatings.Count > 0 ? Math.Round(courseRatings.Average(), 1) : 0.0;

            // Monthly trends (last 6 months)
            var enrollmentTrends = new List<MonthlyTrendDto>();
            var revenueTrends = new List<MonthlyTrendDto>();

            var now = DateTimeOffset.UtcNow;
            for (int i = 5; i >= 0; i--)
            {
                var targetMonth = now.AddMonths(-i);
                int month = targetMonth.Month;
                int year = targetMonth.Year;
                string monthName = targetMonth.ToString("MMM");

                int mEnrollments = courseEnrollments.Count(e => e.CreatedAtUtc.Month == month && e.CreatedAtUtc.Year == year);
                decimal mRevenue = paidOrders
                    .Where(o => o.CreatedAtUtc.Month == month && o.CreatedAtUtc.Year == year)
                    .SelectMany(o => o.Items)
                    .Where(item => item.CourseId == course.Id)
                    .Sum(item => item.UnitPriceSnapshot.Amount);

                enrollmentTrends.Add(new MonthlyTrendDto(monthName, year, mEnrollments, 0m));
                revenueTrends.Add(new MonthlyTrendDto(monthName, year, 0, mRevenue));
            }

            analyticsList.Add(new InstructorCourseAnalyticsDto(
                CourseId: course.Id,
                CourseTitle: course.Title,
                Status: course.Status,
                TotalStudents: totalStudents,
                ActiveStudents: activeStudents,
                CompletedStudents: completedStudents,
                AvgCompletionPercentage: avgProgress,
                TotalRevenue: courseRevenue,
                AverageRating: avgRating,
                RatingCount: courseRatings.Count,
                EnrollmentTrends: enrollmentTrends,
                RevenueTrends: revenueTrends));
        }

        return analyticsList;
    }
}
