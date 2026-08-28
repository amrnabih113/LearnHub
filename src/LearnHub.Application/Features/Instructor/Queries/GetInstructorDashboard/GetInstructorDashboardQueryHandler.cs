using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Instructor.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Purchasing.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Instructor.Queries.GetInstructorDashboard;

public sealed class GetInstructorDashboardQueryHandler(IAppDbContext context)
    : IRequestHandler<GetInstructorDashboardQuery, Result<InstructorDashboardDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<InstructorDashboardDto>> Handle(
        GetInstructorDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var instructorCourses = await _context.Courses
            .AsNoTracking()
            .Where(c => c.InstructorId == request.InstructorId)
            .ToListAsync(cancellationToken);

        int totalCourses = instructorCourses.Count;
        int publishedCourses = instructorCourses.Count(c => c.Status == CourseStatus.Published);
        int draftCourses = instructorCourses.Count(c => c.Status == CourseStatus.Draft);
        int archivedCourses = instructorCourses.Count(c => c.Status == CourseStatus.Archived);

        var courseIds = instructorCourses.Select(c => c.Id).ToList();

        if (courseIds.Count == 0)
        {
            return new InstructorDashboardDto(
                0, 0, 0, 0, 0, 0m, 0m, "USD", [], [], []);
        }

        // Total distinct enrolled students in instructor's courses
        var totalEnrolledStudents = await _context.Enrollments
            .AsNoTracking()
            .Where(e => courseIds.Contains(e.CourseId))
            .Select(e => e.StudentId)
            .Distinct()
            .CountAsync(cancellationToken);

        // Instructor Revenue calculation from paid orders
        var paidOrders = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Paid && o.Items.Any(i => courseIds.Contains(i.CourseId)))
            .ToListAsync(cancellationToken);

        decimal totalRevenue = paidOrders
            .SelectMany(o => o.Items)
            .Where(i => courseIds.Contains(i.CourseId))
            .Sum(i => i.UnitPriceSnapshot.Amount);

        var now = DateTimeOffset.UtcNow;
        var startOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);

        decimal currentMonthRevenue = paidOrders
            .Where(o => o.CreatedAtUtc >= startOfMonth)
            .SelectMany(o => o.Items)
            .Where(i => courseIds.Contains(i.CourseId))
            .Sum(i => i.UnitPriceSnapshot.Amount);

        // Top Performing Courses
        var topCourseDtos = new List<InstructorCoursePerformanceDto>();

        foreach (var c in instructorCourses)
        {
            int enrolledCount = await _context.Enrollments
                .AsNoTracking()
                .CountAsync(e => e.CourseId == c.Id, cancellationToken);

            decimal cRevenue = paidOrders
                .SelectMany(o => o.Items)
                .Where(i => i.CourseId == c.Id)
                .Sum(i => i.UnitPriceSnapshot.Amount);

            var ratings = await _context.CourseReviews
                .AsNoTracking()
                .Where(r => r.CourseId == c.Id && r.Status == Domain.Reviews.Enums.ReviewStatus.Published)
                .Select(r => (double?)r.Rating.Value)
                .ToListAsync(cancellationToken);

            double avgRating = ratings.Count > 0 ? Math.Round(ratings.Average() ?? 0.0, 1) : 0.0;

            topCourseDtos.Add(new InstructorCoursePerformanceDto(
                CourseId: c.Id,
                Title: c.Title,
                ThumbnailUrl: c.ThumbnailUrl,
                Status: c.Status,
                EnrolledStudents: enrolledCount,
                Revenue: cRevenue,
                AverageRating: avgRating,
                RatingCount: ratings.Count));
        }

        topCourseDtos = topCourseDtos
            .OrderByDescending(x => x.Revenue)
            .ThenByDescending(x => x.EnrolledStudents)
            .Take(5)
            .ToList();

        // Recent Enrollments
        var recentEnrollmentsRaw = await _context.Enrollments
            .AsNoTracking()
            .Where(e => courseIds.Contains(e.CourseId))
            .OrderByDescending(e => e.CreatedAtUtc)
            .Take(5)
            .ToListAsync(cancellationToken);

        var studentIds = recentEnrollmentsRaw.Select(e => e.StudentId).Distinct().ToList();
        var studentNames = await _context.Users
            .AsNoTracking()
            .Where(u => studentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}", cancellationToken);

        var courseTitles = instructorCourses.ToDictionary(c => c.Id, c => c.Title);

        var recentEnrollments = recentEnrollmentsRaw.Select(e => new RecentInstructorEnrollmentDto(
            EnrollmentId: e.Id,
            StudentId: e.StudentId,
            StudentName: studentNames.TryGetValue(e.StudentId, out var sName) ? sName : "Student",
            CourseId: e.CourseId,
            CourseTitle: courseTitles.TryGetValue(e.CourseId, out var cTitle) ? cTitle : "Course",
            EnrolledAtUtc: e.CreatedAtUtc)).ToList();

        // Recent Paid Orders
        var recentOrdersRaw = paidOrders
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(5)
            .ToList();

        var recentOrderStudentIds = recentOrdersRaw.Select(o => o.StudentId).Distinct().ToList();
        var orderStudentNames = await _context.Users
            .AsNoTracking()
            .Where(u => recentOrderStudentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}", cancellationToken);

        var recentOrders = new List<RecentInstructorOrderDto>();
        foreach (var o in recentOrdersRaw)
        {
            var firstItem = o.Items.FirstOrDefault(i => courseIds.Contains(i.CourseId));
            if (firstItem is null) continue;

            recentOrders.Add(new RecentInstructorOrderDto(
                OrderId: o.Id,
                StudentId: o.StudentId,
                StudentName: orderStudentNames.TryGetValue(o.StudentId, out var name) ? name : "Student",
                CourseId: firstItem.CourseId,
                CourseTitle: courseTitles.TryGetValue(firstItem.CourseId, out var t) ? t : "Course",
                Amount: firstItem.UnitPriceSnapshot.Amount,
                Currency: firstItem.UnitPriceSnapshot.Currency,
                PaidAtUtc: o.CreatedAtUtc));
        }

        return new InstructorDashboardDto(
            TotalCourses: totalCourses,
            PublishedCourses: publishedCourses,
            DraftCourses: draftCourses,
            ArchivedCourses: archivedCourses,
            TotalEnrolledStudents: totalEnrolledStudents,
            TotalRevenue: totalRevenue,
            CurrentMonthRevenue: currentMonthRevenue,
            Currency: "USD",
            TopCourses: topCourseDtos,
            RecentEnrollments: recentEnrollments,
            RecentOrders: recentOrders);
    }
}
