using LearnHub.Domain.Enrollments.Enums;

namespace LearnHub.Application.Features.Student.Dtos;

public sealed record StudentProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? ImageUrl,
    DateOnly? DateOfBirth,
    string? Bio,
    string? Country,
    bool IsEmailVerified,
    IReadOnlyList<string> Roles,
    DateTimeOffset CreatedAtUtc);

public sealed record StudentLearningDashboardDto(
    IReadOnlyList<EnrolledCourseProgressDto> Courses,
    int TotalEnrolled,
    int ActiveCount,
    int CompletedCount,
    int PausedCount);

public sealed record EnrolledCourseProgressDto(
    Guid EnrollmentId,
    Guid CourseId,
    string CourseTitle,
    string? ThumbnailUrl,
    string CategoryName,
    string InstructorName,
    decimal ProgressPercentage,
    EnrollmentStatus Status,
    DateTimeOffset? CompletedAtUtc,
    DateTimeOffset LastAccessedAtUtc,
    bool IsAccessible,
    bool CanWatchLessons,
    Guid? NextLessonId,
    string? NextLessonTitle);

public sealed record StudentStatisticsDto(
    int LearningTimeThisWeekMinutes,
    int EnrolledCourses,
    int CurrentStreakDays,
    int LongestStreakDays,
    int Certificates,
    DateTimeOffset? LastLearningActivityUtc,
    IReadOnlyList<DailyActivityDto> WeeklyActivity);

public sealed record DailyActivityDto(
    string DayOfWeek,
    int MinutesLearned,
    DateTime Date);

public sealed record StudentOrderDto(
    Guid OrderId,
    DateTimeOffset OrderDate,
    string Status,
    decimal TotalAmount,
    string Currency,
    int ItemsCount,
    IReadOnlyList<string> CourseTitles);

public sealed record StudentOrderDetailDto(
    Guid OrderId,
    DateTimeOffset OrderDate,
    string Status,
    decimal TotalAmount,
    string Currency,
    IReadOnlyList<StudentOrderItemDto> Items);

public sealed record StudentOrderItemDto(
    Guid CourseId,
    string CourseTitle,
    decimal Price,
    string Currency);
