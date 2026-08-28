using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Certificates.Queries.GetStudentCertificates;

public sealed class GetStudentCertificatesQueryHandler(IAppDbContext context)
    : IRequestHandler<GetStudentCertificatesQuery, Result<IReadOnlyList<CertificateDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<IReadOnlyList<CertificateDto>>> Handle(
        GetStudentCertificatesQuery request,
        CancellationToken cancellationToken)
    {
        var certificates = await _context.Certificates
            .AsNoTracking()
            .Where(c => c.StudentId == request.StudentId)
            .OrderByDescending(c => c.IssuedAtUtc)
            .Select(c => new CertificateDto(
                c.Id,
                c.Code,
                c.EnrollmentId,
                c.StudentId,
                c.StudentName,
                c.CourseId,
                c.CourseName,
                c.InstructorName,
                c.PdfUrl,
                c.IssuedAtUtc,
                c.IsRevoked,
                c.RevokedAtUtc,
                c.RevocationReason))
            .ToListAsync(cancellationToken);

        return certificates;
    }
}
