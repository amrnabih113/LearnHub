namespace LearnHub.Application.Features.Admin.Dtos;

public sealed record AdminDashboardDto(
    string DateRangeFilter,
    UserMetricsDto Users,
    CourseMetricsDto Courses,
    EnrollmentMetricsDto Enrollments,
    OrderMetricsDto Orders,
    PaymentMetricsDto Payments,
    SubscriptionMetricsDto Subscriptions,
    ReviewMetricsDto Reviews,
    RecentActivityDto RecentActivity);

public sealed record UserMetricsDto(
    int TotalUsers,
    int Students,
    int Instructors,
    int Admins,
    int VerifiedUsers);

public sealed record CourseMetricsDto(
    int TotalCourses,
    int Published,
    int Draft,
    int Archived);

public sealed record EnrollmentMetricsDto(
    int TotalEnrollments,
    int Active,
    int Completed);

public sealed record OrderMetricsDto(
    int TotalOrders,
    int PaidOrders,
    int PendingOrders,
    int CancelledOrders);

public sealed record PaymentMetricsDto(
    int TotalSuccessfulPayments,
    decimal TotalRevenue,
    decimal CurrentMonthRevenue,
    string Currency);

public sealed record SubscriptionMetricsDto(
    int ActiveSubscriptions,
    int ExpiredOrCancelledSubscriptions);

public sealed record ReviewMetricsDto(
    int TotalReviews,
    double AverageRating);

public sealed record RecentActivityDto(
    IReadOnlyList<RecentOrderDto> RecentOrders,
    IReadOnlyList<RecentEnrollmentDto> RecentEnrollments,
    IReadOnlyList<RecentCourseDto> RecentCourses);

public sealed record RecentOrderDto(
    Guid OrderId,
    Guid StudentId,
    string StudentName,
    decimal TotalAmount,
    string Status,
    DateTimeOffset CreatedAtUtc);

public sealed record RecentEnrollmentDto(
    Guid EnrollmentId,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    DateTimeOffset EnrolledAtUtc);

public sealed record RecentCourseDto(
    Guid CourseId,
    string Title,
    Guid InstructorId,
    string InstructorName,
    string Status,
    DateTimeOffset CreatedAtUtc);
