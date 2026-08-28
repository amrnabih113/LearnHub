using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Certificates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Certificates.Queries.VerifyCertificate;

public sealed class VerifyCertificateQueryHandler(IAppDbContext context)
    : IRequestHandler<VerifyCertificateQuery, Result<CertificateVerificationDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CertificateVerificationDto>> Handle(
        VerifyCertificateQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return CertificateErrors.CodeRequired;
        }

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var cert = await _context.Certificates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);

        if (cert is null)
        {
            return CertificateErrors.CertificateNotFound;
        }

        if (cert.IsRevoked)
        {
            return new CertificateVerificationDto(
                IsValid: false,
                Code: cert.Code,
                StudentName: cert.StudentName,
                CourseTitle: cert.CourseName,
                InstructorName: cert.InstructorName,
                IssuedAtUtc: cert.IssuedAtUtc,
                Status: "Revoked",
                RevokedAtUtc: cert.RevokedAtUtc,
                RevocationReason: cert.RevocationReason);
        }

        return new CertificateVerificationDto(
            IsValid: true,
            Code: cert.Code,
            StudentName: cert.StudentName,
            CourseTitle: cert.CourseName,
            InstructorName: cert.InstructorName,
            IssuedAtUtc: cert.IssuedAtUtc,
            Status: "Valid");
    }
}
