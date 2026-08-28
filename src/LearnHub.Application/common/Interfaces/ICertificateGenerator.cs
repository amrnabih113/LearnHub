using LearnHub.Domain.Common.Results;

namespace LearnHub.Application.common.Interfaces;

public sealed record CertificatePdfModel(
    string CertificateCode,
    string StudentName,
    string CourseTitle,
    string InstructorName,
    DateTimeOffset IssuedAtUtc,
    string VerificationUrl);

public interface ICertificateGenerator
{
    Task<Result<byte[]>> GeneratePdfAsync(
        CertificatePdfModel model,
        CancellationToken cancellationToken = default);
}
