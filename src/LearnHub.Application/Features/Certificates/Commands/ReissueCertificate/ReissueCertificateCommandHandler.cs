using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Options;
using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Certificates;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LearnHub.Application.Features.Certificates.Commands.ReissueCertificate;

public sealed class ReissueCertificateCommandHandler(
    IAppDbContext context,
    ICertificateGenerator pdfGenerator,
    IFileStorageService storageService,
    IOptions<CertificateOptions> certOptions)
    : IRequestHandler<ReissueCertificateCommand, Result<CertificateDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICertificateGenerator _pdfGenerator = pdfGenerator;
    private readonly IFileStorageService _storageService = storageService;
    private readonly CertificateOptions _options = certOptions.Value;

    public async Task<Result<CertificateDto>> Handle(
        ReissueCertificateCommand request,
        CancellationToken cancellationToken)
    {
        var cert = await _context.Certificates
            .FirstOrDefaultAsync(c => c.Id == request.CertificateId, cancellationToken);

        if (cert is null)
        {
            return CertificateErrors.CertificateNotFound;
        }

        var student = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == cert.StudentId, cancellationToken);

        var course = await _context.Courses
            .Include(c => c.Instructor)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cert.CourseId, cancellationToken);

        var studentName = student?.FullName ?? cert.StudentName;
        var courseTitle = course?.Title ?? cert.CourseName;
        var instructorName = course?.Instructor?.FullName ?? cert.InstructorName;

        var verificationUrl = $"{_options.VerificationBaseUrl.TrimEnd('/')}/{cert.Code}";

        var pdfModel = new CertificatePdfModel(
            cert.Code,
            studentName,
            courseTitle,
            instructorName,
            cert.IssuedAtUtc,
            verificationUrl);

        var pdfResult = await _pdfGenerator.GeneratePdfAsync(pdfModel, cancellationToken);
        if (pdfResult.IsError)
        {
            return pdfResult.Errors;
        }

        var fileName = $"{cert.Code}.pdf";
        var uploadResult = await _storageService.UploadRawFileAsync(
            pdfResult.Value, fileName, "certificates", "application/pdf", cancellationToken);

        var pdfUrl = uploadResult.IsSuccess ? uploadResult.Value : cert.PdfUrl;

        if (!string.IsNullOrWhiteSpace(pdfUrl))
        {
            cert.UpdateUrls(pdfUrl);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new CertificateDto(
            cert.Id,
            cert.Code,
            cert.EnrollmentId,
            cert.StudentId,
            studentName,
            cert.CourseId,
            courseTitle,
            instructorName,
            cert.PdfUrl,
            cert.IssuedAtUtc,
            cert.IsRevoked,
            cert.RevokedAtUtc,
            cert.RevocationReason);
    }
}
