namespace LearnHub.Application.Features.Enrollments.Dtos;

public sealed record CourseEntitlementsDto(
    bool HasPurchase,
    bool HasValidSubscription,
    bool IsFreeCourse,
    bool IsAdminGranted,
    bool IsCompleted);
