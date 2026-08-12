namespace LearnHub.Application.Features.Enrollments.Dtos;

public sealed record CertificateDto(
    Guid Id,
    Guid EnrollmentId,
    Guid StudentId,
    string CertificateCode,
    DateTimeOffset IssuedAtUtc);
