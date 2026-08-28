namespace LearnHub.Contracts.Certificates.Responses;

public sealed record CertificateResponse(
    Guid Id,
    string Code,
    Guid EnrollmentId,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    string InstructorName,
    string? PdfUrl,
    DateTimeOffset IssuedAtUtc);
