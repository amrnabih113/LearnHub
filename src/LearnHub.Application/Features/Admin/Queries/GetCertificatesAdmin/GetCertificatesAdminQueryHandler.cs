using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetCertificatesAdmin;

public sealed class GetCertificatesAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCertificatesAdminQuery, Result<PagedResult<CertificateDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<CertificateDto>>> Handle(
        GetCertificatesAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Certificates.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(c => c.Code.ToLower().Contains(search)
                                  || c.StudentName.ToLower().Contains(search)
                                  || c.CourseName.ToLower().Contains(search));
        }

        if (request.CourseId.HasValue)
        {
            query = query.Where(c => c.CourseId == request.CourseId.Value);
        }

        if (request.StudentId.HasValue)
        {
            query = query.Where(c => c.StudentId == request.StudentId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(c => c.IssuedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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

        return new PagedResult<CertificateDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
