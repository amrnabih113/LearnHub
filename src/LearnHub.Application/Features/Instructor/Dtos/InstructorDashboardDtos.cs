using LearnHub.Domain.Courses.Enums;

namespace LearnHub.Application.Features.Instructor.Dtos;

public sealed record InstructorDashboardDto(
    int TotalCourses,
    int PublishedCourses,
    int DraftCourses,
    int ArchivedCourses,
    int TotalEnrolledStudents,
    decimal TotalRevenue,
    decimal CurrentMonthRevenue,
    string Currency,
    IReadOnlyList<InstructorCoursePerformanceDto> TopCourses,
    IReadOnlyList<RecentInstructorEnrollmentDto> RecentEnrollments,
    IReadOnlyList<RecentInstructorOrderDto> RecentOrders);

public sealed record InstructorCoursePerformanceDto(
    Guid CourseId,
    string Title,
    string? ThumbnailUrl,
    CourseStatus Status,
    int EnrolledStudents,
    decimal Revenue,
    double AverageRating,
    int RatingCount);

public sealed record RecentInstructorEnrollmentDto(
    Guid EnrollmentId,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    DateTimeOffset EnrolledAtUtc);

public sealed record RecentInstructorOrderDto(
    Guid OrderId,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    decimal Amount,
    string Currency,
    DateTimeOffset PaidAtUtc);

public sealed record InstructorCourseAnalyticsDto(
    Guid CourseId,
    string CourseTitle,
    CourseStatus Status,
    int TotalStudents,
    int ActiveStudents,
    int CompletedStudents,
    decimal AvgCompletionPercentage,
    decimal TotalRevenue,
    double AverageRating,
    int RatingCount,
    IReadOnlyList<MonthlyTrendDto> EnrollmentTrends,
    IReadOnlyList<MonthlyTrendDto> RevenueTrends);

public sealed record MonthlyTrendDto(
    string Month,
    int Year,
    int Count,
    decimal Amount);
