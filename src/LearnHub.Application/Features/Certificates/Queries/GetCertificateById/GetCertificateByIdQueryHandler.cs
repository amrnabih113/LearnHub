using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Certificates;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Certificates.Queries.GetCertificateById;

public sealed class GetCertificateByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCertificateByIdQuery, Result<CertificateDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<CertificateDto>> Handle(
        GetCertificateByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cert = await _context.Certificates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CertificateId, cancellationToken);

        if (cert is null)
        {
            return CertificateErrors.CertificateNotFound;
        }

        // Ownership & Admin check
        if (!request.IsAdmin && cert.StudentId != request.StudentId)
        {
            return CertificateErrors.UnauthorizedAccess;
        }

        return new CertificateDto(
            cert.Id,
            cert.Code,
            cert.EnrollmentId,
            cert.StudentId,
            cert.StudentName,
            cert.CourseId,
            cert.CourseName,
            cert.InstructorName,
            cert.PdfUrl,
            cert.IssuedAtUtc,
            cert.IsRevoked,
            cert.RevokedAtUtc,
            cert.RevocationReason);
    }
}
