using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Enrollments.Certificates;

public sealed class Certificate : AuditableEntity
{
    public Guid EnrollmentId { get; private set; }

    public Enrollment Enrollment { get; private set; } = default!;
    public string StudentId  { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public DateTimeOffset IssuedAtUtc { get; private set; }

    private Certificate() { }

    private Certificate(Guid id, Guid enrollmentId, string studentId, string code) : base(id)
    {
        EnrollmentId = enrollmentId;
        StudentId = studentId;
        Code = code;
        IssuedAtUtc = DateTimeOffset.UtcNow;
    }

    public static Result<Certificate> Create(Guid id, Guid enrollmentId, string studentId,   string code)
    {
        if (enrollmentId == Guid.Empty)
        {
            return CertificateErrors.EnrollmentIdRequired;
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return CertificateErrors.CodeRequired;
        }

        return new Certificate(id, enrollmentId, studentId,    code);
    }
}
