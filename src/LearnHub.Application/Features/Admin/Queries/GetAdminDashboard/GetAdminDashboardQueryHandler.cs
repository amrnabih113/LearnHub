using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Enrollments.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Purchasing.Enums;
using LearnHub.Domain.Reviews.Enums;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetAdminDashboard;

public sealed class GetAdminDashboardQueryHandler(IAppDbContext context)
    : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<AdminDashboardDto>> Handle(
        GetAdminDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        DateTimeOffset? startDate = request.Range?.ToLowerInvariant() switch
        {
            "today" => new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero),
            "thisweek" => now.AddDays(-(int)now.DayOfWeek),
            "thismonth" => new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero),
            _ => null
        };

        var rangeLabel = request.Range?.ToLowerInvariant() switch
        {
            "today" => "Today",
            "thisweek" => "This Week",
            "thismonth" => "This Month",
            _ => "All Time"
        };

        // Users metrics
        var userQuery = _context.Users.AsNoTracking();
        if (startDate.HasValue) userQuery = userQuery.Where(u => u.CreatedAtUtc >= startDate.Value);

        var totalUsers = await userQuery.CountAsync(cancellationToken);
        var studentsCount = await userQuery.CountAsync(u => u.Roles.Any(r => r.Role == Role.Student), cancellationToken);
        var instructorsCount = await userQuery.CountAsync(u => u.Roles.Any(r => r.Role == Role.Instructor), cancellationToken);
        var adminsCount = await userQuery.CountAsync(u => u.Roles.Any(r => r.Role == Role.Admin), cancellationToken);
        var verifiedUsersCount = await userQuery.CountAsync(u => u.IsEmailVerified, cancellationToken);

        var userMetrics = new UserMetricsDto(totalUsers, studentsCount, instructorsCount, adminsCount, verifiedUsersCount);

        // Course metrics
        var courseQuery = _context.Courses.AsNoTracking();
        if (startDate.HasValue) courseQuery = courseQuery.Where(c => c.CreatedAtUtc >= startDate.Value);

        var totalCourses = await courseQuery.CountAsync(cancellationToken);
        var publishedCourses = await courseQuery.CountAsync(c => c.Status == CourseStatus.Published, cancellationToken);
        var draftCourses = await courseQuery.CountAsync(c => c.Status == CourseStatus.Draft, cancellationToken);
        var archivedCourses = await courseQuery.CountAsync(c => c.Status == CourseStatus.Archived, cancellationToken);

        var courseMetrics = new CourseMetricsDto(totalCourses, publishedCourses, draftCourses, archivedCourses);

        // Enrollment metrics
        var enrollmentQuery = _context.Enrollments.AsNoTracking();
        if (startDate.HasValue) enrollmentQuery = enrollmentQuery.Where(e => e.CreatedAtUtc >= startDate.Value);

        var totalEnrollments = await enrollmentQuery.CountAsync(cancellationToken);
        var activeEnrollments = await enrollmentQuery.CountAsync(e => e.Status == EnrollmentStatus.Active, cancellationToken);
        var completedEnrollments = await enrollmentQuery.CountAsync(e => e.Status == EnrollmentStatus.Completed, cancellationToken);

        var enrollmentMetrics = new EnrollmentMetricsDto(totalEnrollments, activeEnrollments, completedEnrollments);

        // Order metrics
        var orderQuery = _context.Orders.AsNoTracking();
        if (startDate.HasValue) orderQuery = orderQuery.Where(o => o.CreatedAtUtc >= startDate.Value);

        var totalOrders = await orderQuery.CountAsync(cancellationToken);
        var paidOrders = await orderQuery.CountAsync(o => o.Status == OrderStatus.Paid, cancellationToken);
        var pendingOrders = await orderQuery.CountAsync(o => o.Status == OrderStatus.PendingPayment, cancellationToken);
        var cancelledOrders = await orderQuery.CountAsync(o => o.Status == OrderStatus.Cancelled || o.Status == OrderStatus.Refunded, cancellationToken);

        var orderMetrics = new OrderMetricsDto(totalOrders, paidOrders, pendingOrders, cancelledOrders);

        // Revenue metrics
        var paymentQuery = _context.Payments.AsNoTracking().Where(p => p.Status == Domain.Common.Enums.PaymentStatus.Succeeded);
        if (startDate.HasValue) paymentQuery = paymentQuery.Where(p => p.CreatedAtUtc >= startDate.Value);

        var totalSuccessfulPayments = await paymentQuery.CountAsync(cancellationToken);
        var totalRevenue = await paymentQuery.SumAsync(p => (decimal?)p.Amount.Amount, cancellationToken) ?? 0m;

        var startOfCurrentMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var currentMonthRevenue = await _context.Payments
            .AsNoTracking()
            .Where(p => p.Status == Domain.Common.Enums.PaymentStatus.Succeeded && p.CreatedAtUtc >= startOfCurrentMonth)
            .SumAsync(p => (decimal?)p.Amount.Amount, cancellationToken) ?? 0m;

        var paymentMetrics = new PaymentMetricsDto(totalSuccessfulPayments, totalRevenue, currentMonthRevenue, "USD");

        // Subscription metrics
        var subQuery = _context.Subscriptions.AsNoTracking();
        if (startDate.HasValue) subQuery = subQuery.Where(s => s.CreatedAtUtc >= startDate.Value);

        var activeSubs = await subQuery.CountAsync(s => s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trialing, cancellationToken);
        var expiredOrCancelledSubs = await subQuery.CountAsync(s => s.Status == SubscriptionStatus.Cancelled || s.Status == SubscriptionStatus.Expired, cancellationToken);

        var subscriptionMetrics = new SubscriptionMetricsDto(activeSubs, expiredOrCancelledSubs);

        // Review metrics
        var reviewQuery = _context.CourseReviews.AsNoTracking().Where(r => r.Status == ReviewStatus.Published);
        if (startDate.HasValue) reviewQuery = reviewQuery.Where(r => r.CreatedAtUtc >= startDate.Value);

        var totalReviews = await reviewQuery.CountAsync(cancellationToken);
        var ratingsList = await reviewQuery.Select(r => r.Rating.Value).ToListAsync(cancellationToken);
        var avgRating = ratingsList.Count > 0 ? Math.Round(ratingsList.Average(), 1) : 0.0;

        var reviewMetrics = new ReviewMetricsDto(totalReviews, avgRating);

        // Recent Activity
        var recentOrdersRaw = await _context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(5)
            .ToListAsync(cancellationToken);

        var recentOrderStudentIds = recentOrdersRaw.Select(o => o.StudentId).Distinct().ToList();
        var recentOrderStudents = await _context.Users
            .AsNoTracking()
            .Where(u => recentOrderStudentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        var recentOrders = recentOrdersRaw.Select(o => new RecentOrderDto(
            o.Id,
            o.StudentId,
            recentOrderStudents.TryGetValue(o.StudentId, out var sName) ? sName : string.Empty,
            o.TotalAmount.Amount,
            o.Status.ToString(),
            o.CreatedAtUtc)).ToList();

        var recentEnrollmentsRaw = await _context.Enrollments
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedAtUtc)
            .Take(5)
            .ToListAsync(cancellationToken);

        var recentEnrollmentStudentIds = recentEnrollmentsRaw.Select(e => e.StudentId).Distinct().ToList();
        var recentEnrollmentStudents = await _context.Users
            .AsNoTracking()
            .Where(u => recentEnrollmentStudentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        var recentEnrollmentCourseIds = recentEnrollmentsRaw.Select(e => e.CourseId).Distinct().ToList();
        var recentEnrollmentCourses = await _context.Courses
            .AsNoTracking()
            .Where(c => recentEnrollmentCourseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Title, cancellationToken);

        var recentEnrollments = recentEnrollmentsRaw.Select(e => new RecentEnrollmentDto(
            e.Id,
            e.StudentId,
            recentEnrollmentStudents.TryGetValue(e.StudentId, out var eName) ? eName : string.Empty,
            e.CourseId,
            recentEnrollmentCourses.TryGetValue(e.CourseId, out var cTitle) ? cTitle : string.Empty,
            e.CreatedAtUtc)).ToList();

        var recentCoursesRaw = await _context.Courses
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAtUtc)
            .Take(5)
            .ToListAsync(cancellationToken);

        var recentCourseInstructorIds = recentCoursesRaw.Where(c => c.InstructorId.HasValue).Select(c => c.InstructorId!.Value).Distinct().ToList();
        var recentCourseInstructors = await _context.Users
            .AsNoTracking()
            .Where(u => recentCourseInstructorIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

        var recentCourses = recentCoursesRaw.Select(c => new RecentCourseDto(
            c.Id,
            c.Title,
            c.InstructorId ?? Guid.Empty,
            c.InstructorId.HasValue && recentCourseInstructors.TryGetValue(c.InstructorId.Value, out var iName) ? iName : string.Empty,
            c.Status.ToString(),
            c.CreatedAtUtc)).ToList();

        var recentActivity = new RecentActivityDto(recentOrders, recentEnrollments, recentCourses);

        return new AdminDashboardDto(
            rangeLabel,
            userMetrics,
            courseMetrics,
            enrollmentMetrics,
            orderMetrics,
            paymentMetrics,
            subscriptionMetrics,
            reviewMetrics,
            recentActivity);
    }
}
