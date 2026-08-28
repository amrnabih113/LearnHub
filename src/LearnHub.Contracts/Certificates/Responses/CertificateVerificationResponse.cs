namespace LearnHub.Contracts.Certificates.Responses;

public sealed record CertificateVerificationResponse(
    bool IsValid,
    string Code,
    string StudentName,
    string CourseTitle,
    string InstructorName,
    DateTimeOffset IssuedAtUtc,
    string Status,
    DateTimeOffset? RevokedAtUtc = null,
    string? RevocationReason = null);
