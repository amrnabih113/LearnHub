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

    public static Error EnrollmentNotCompleted
    => Error.Validation(code: "DomainError.Certificate.EnrollmentNotCompleted",
    description: "Certificate can only be issued for completed enrollments.");

    public static Error AlreadyIssued
    => Error.Conflict(code: "DomainError.Certificate.AlreadyIssued",
    description: "Certificate has already been issued for this enrollment.");

    public static Error CertificateNotFound
    => Error.NotFound(code: "DomainError.Certificate.NotFound",
    description: "Certificate was not found.");

    public static Error GenerationFailed
    => Error.Failure(code: "DomainError.Certificate.GenerationFailed",
    description: "Failed to generate certificate file.");

    public static Error UnauthorizedAccess
    => Error.Forbidden(code: "DomainError.Certificate.UnauthorizedAccess",
    description: "You do not have access to this certificate.");

    public static Error AlreadyRevoked
    => Error.Conflict(code: "DomainError.Certificate.AlreadyRevoked",
    description: "Certificate is already revoked.");

    public static Error RevocationReasonRequired
    => Error.Validation(code: "DomainError.Certificate.RevocationReasonRequired",
    description: "Revocation reason is required.");
}
