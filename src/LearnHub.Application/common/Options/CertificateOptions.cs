namespace LearnHub.Application.common.Options;

public sealed class CertificateOptions
{
    public const string SectionName = "Certificate";

    public string VerificationBaseUrl { get; set; } = "https://learnhub.com/verify/certificates";
    public string OrganizationName { get; set; } = "LearnHub Academy";
}
