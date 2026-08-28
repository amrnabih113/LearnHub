using LearnHub.Domain.Common;
using LearnHub.Domain.Common.Results;

namespace LearnHub.Domain.Instructor;

public sealed class InstructorCertification : AuditableEntity
{
    public Guid InstructorProfileId { get; private set; }
    public string Name { get; private set; } = default!;
    public string IssuingOrganization { get; private set; } = default!;
    public DateOnly IssueDate { get; private set; }
    public DateOnly? ExpirationDate { get; private set; }
    public string? CredentialId { get; private set; }
    public string? CredentialUrl { get; private set; }

    private InstructorCertification() { }

    private InstructorCertification(
        Guid id,
        Guid instructorProfileId,
        string name,
        string issuingOrganization,
        DateOnly issueDate,
        DateOnly? expirationDate,
        string? credentialId,
        string? credentialUrl) : base(id)
    {
        InstructorProfileId = instructorProfileId;
        Name = name;
        IssuingOrganization = issuingOrganization;
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        CredentialId = credentialId;
        CredentialUrl = credentialUrl;
    }

    public static Result<InstructorCertification> Create(
        Guid id,
        Guid instructorProfileId,
        string name,
        string issuingOrganization,
        DateOnly issueDate,
        DateOnly? expirationDate,
        string? credentialId,
        string? credentialUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("InstructorCertification.NameRequired", "Certification name is required.");
        }
        if (string.IsNullOrWhiteSpace(issuingOrganization))
        {
            return Error.Validation("InstructorCertification.OrganizationRequired", "Issuing organization is required.");
        }

        if (!string.IsNullOrWhiteSpace(credentialUrl))
        {
            if (!Uri.TryCreate(credentialUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return Error.Validation("InstructorCertification.InvalidUrl", "Credential URL must be a valid HTTP or HTTPS URL.");
            }
        }

        return new InstructorCertification(
            id, instructorProfileId, name.Trim(), issuingOrganization.Trim(),
            issueDate, expirationDate, credentialId?.Trim(), credentialUrl?.Trim());
    }
}
