using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Enrollments.Certificates;

public sealed class Certificate : AuditableEntity
{
    public Guid EnrollmentId { get; private set; }
    public Enrollment Enrollment { get; private set; } = default!;
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public string Code { get; private set; } = default!;
    public string StudentName { get; private set; } = default!;
    public string CourseName { get; private set; } = default!;
    public string InstructorName { get; private set; } = default!;
    public string? PdfUrl { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTimeOffset IssuedAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }
    public string? RevocationReason { get; private set; }

    private Certificate() { }

    private Certificate(
        Guid id,
        Guid enrollmentId,
        Guid studentId,
        Guid courseId,
        string code,
        string studentName,
        string courseName,
        string instructorName,
        string? pdfUrl = null,
        string? imageUrl = null) : base(id)
    {
        EnrollmentId = enrollmentId;
        StudentId = studentId;
        CourseId = courseId;
        Code = code;
        StudentName = studentName;
        CourseName = courseName;
        InstructorName = instructorName;
        PdfUrl = pdfUrl;
        ImageUrl = imageUrl;
        IssuedAtUtc = DateTimeOffset.UtcNow;
    }

    public static Result<Certificate> Create(
        Guid id,
        Guid enrollmentId,
        Guid studentId,
        Guid courseId,
        string code,
        string studentName,
        string courseName,
        string instructorName,
        string? pdfUrl = null,
        string? imageUrl = null)
    {
        if (enrollmentId == Guid.Empty)
        {
            return CertificateErrors.EnrollmentIdRequired;
        }

        if (studentId == Guid.Empty)
        {
            return Error.Validation("Certificate.StudentIdRequired", "Student ID is required.");
        }

        if (courseId == Guid.Empty)
        {
            return Error.Validation("Certificate.CourseIdRequired", "Course ID is required.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return CertificateErrors.CodeRequired;
        }

        if (string.IsNullOrWhiteSpace(studentName))
        {
            return Error.Validation("Certificate.StudentNameRequired", "Student name is required.");
        }

        if (string.IsNullOrWhiteSpace(courseName))
        {
            return Error.Validation("Certificate.CourseNameRequired", "Course name is required.");
        }

        return new Certificate(
            id,
            enrollmentId,
            studentId,
            courseId,
            code.Trim().ToUpperInvariant(),
            studentName.Trim(),
            courseName.Trim(),
            instructorName?.Trim() ?? string.Empty,
            pdfUrl?.Trim(),
            imageUrl?.Trim());
    }

    public void UpdateUrls(string pdfUrl, string? imageUrl = null)
    {
        PdfUrl = pdfUrl.Trim();
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            ImageUrl = imageUrl.Trim();
        }
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Result<Updated> Revoke(string reason)
    {
        if (IsRevoked)
        {
            return CertificateErrors.AlreadyRevoked;
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return CertificateErrors.RevocationReasonRequired;
        }

        IsRevoked = true;
        RevokedAtUtc = DateTimeOffset.UtcNow;
        RevocationReason = reason.Trim();
        UpdatedAtUtc = DateTimeOffset.UtcNow;

        return Result.Updated;
    }
}
