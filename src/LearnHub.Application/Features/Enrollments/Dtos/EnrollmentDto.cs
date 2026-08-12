using LearnHub.Domain.Enrollments.Enums;

namespace LearnHub.Application.Features.Enrollments.Dtos;

public sealed record EnrollmentDto(
    Guid Id,
    Guid StudentId,
    string? StudentName,
    Guid CourseId,
    string? CourseTitle,
    EnrollmentStatus Status,
    decimal ProgressPercentage,
    DateTimeOffset? CompletedAtUtc,
    DateTimeOffset CreatedAtUtc);
