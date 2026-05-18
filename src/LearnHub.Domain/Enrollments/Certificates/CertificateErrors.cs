using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Enrollments.Certificates;

public static class CertificateErrors
{
    public static Error EnrollmentIdRequired
    => Error.Validation(code: "DomainError.Certificate.EnrollmentIdRequired",
    description: "Enrollment id is required");

    public static Error CodeRequired
    => Error.Validation(code: "DomainError.Certificate.CodeRequired",
    description: "Certificate code is required");
}
