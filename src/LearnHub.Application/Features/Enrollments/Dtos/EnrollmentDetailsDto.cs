using LearnHub.Domain.Enrollments.Enums;

namespace LearnHub.Application.Features.Enrollments.Dtos;

public sealed record EnrollmentDetailsDto(
    Guid Id,
    Guid StudentId,
    string? StudentName,
    Guid CourseId,
    string? CourseTitle,
    EnrollmentStatus Status,
    decimal ProgressPercentage,
    DateTimeOffset? CompletedAtUtc,
    DateTimeOffset CreatedAtUtc,
    CertificateDto? Certificate,
    IReadOnlyCollection<LessonProgressDto> LessonsProgress);
