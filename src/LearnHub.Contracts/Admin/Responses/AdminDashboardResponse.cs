namespace LearnHub.Contracts.Admin.Responses;

public sealed record AdminDashboardResponse(
    string DateRangeFilter,
    UserMetricsResponse Users,
    CourseMetricsResponse Courses,
    EnrollmentMetricsResponse Enrollments,
    OrderMetricsResponse Orders,
    PaymentMetricsResponse Payments,
    SubscriptionMetricsResponse Subscriptions,
    ReviewMetricsResponse Reviews,
    RecentActivityResponse RecentActivity);

public sealed record UserMetricsResponse(
    int TotalUsers,
    int Students,
    int Instructors,
    int Admins,
    int VerifiedUsers);

public sealed record CourseMetricsResponse(
    int TotalCourses,
    int Published,
    int Draft,
    int Archived);

public sealed record EnrollmentMetricsResponse(
    int TotalEnrollments,
    int Active,
    int Completed);

public sealed record OrderMetricsResponse(
    int TotalOrders,
    int PaidOrders,
    int PendingOrders,
    int CancelledOrders);

public sealed record PaymentMetricsResponse(
    int TotalSuccessfulPayments,
    decimal TotalRevenue,
    decimal CurrentMonthRevenue,
    string Currency);

public sealed record SubscriptionMetricsResponse(
    int ActiveSubscriptions,
    int ExpiredOrCancelledSubscriptions);

public sealed record ReviewMetricsResponse(
    int TotalReviews,
    double AverageRating);

public sealed record RecentActivityResponse(
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
