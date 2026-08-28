namespace LearnHub.Application.Features.Certificates.Dtos;

public sealed record CertificateDto(
    Guid Id,
    string Code,
    Guid EnrollmentId,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    string InstructorName,
    string? PdfUrl,
    DateTimeOffset IssuedAtUtc,
    bool IsRevoked = false,
    DateTimeOffset? RevokedAtUtc = null,
    string? RevocationReason = null);

public sealed record CertificateVerificationDto(
    bool IsValid,
    string Code,
    string StudentName,
    string CourseTitle,
    string InstructorName,
    DateTimeOffset IssuedAtUtc,
    string Status,
    DateTimeOffset? RevokedAtUtc = null,
    string? RevocationReason = null);
