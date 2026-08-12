using LearnHub.Domain.Enrollments.Enums;

namespace LearnHub.Application.Features.Enrollments.Dtos;

public sealed record CourseAccessResult(
    Guid CourseId,
    Guid StudentId,
    bool IsAccessible,
    bool CanWatchLessons,
    bool CanViewCertificate,
    EnrollmentStatus? Status,
    decimal ProgressPercentage,
    CourseEntitlementsDto Entitlements);
