using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Options;
using LearnHub.Application.Features.Certificates.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Enrollments.Certificates;
using LearnHub.Domain.Enrollments.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LearnHub.Application.Features.Certificates.Commands.IssueCertificate;

public sealed class IssueCertificateCommandHandler(
    IAppDbContext context,
    ICertificateGenerator pdfGenerator,
    IFileStorageService storageService,
    IOptions<CertificateOptions> certOptions)
    : IRequestHandler<IssueCertificateCommand, Result<CertificateDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICertificateGenerator _pdfGenerator = pdfGenerator;
    private readonly IFileStorageService _storageService = storageService;
    private readonly CertificateOptions _options = certOptions.Value;

    public async Task<Result<CertificateDto>> Handle(
        IssueCertificateCommand request,
        CancellationToken cancellationToken)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Certificate)
            .FirstOrDefaultAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId, cancellationToken);

        if (enrollment is null)
        {
            return Error.NotFound("Enrollment.NotFound", "Enrollment not found.");
        }

        // Must be completed
        if (enrollment.Status != EnrollmentStatus.Completed && enrollment.ProgressPercentage < 100m)
        {
            return CertificateErrors.EnrollmentNotCompleted;
        }

        // Idempotency: Return existing certificate if already issued
        var existingCert = await _context.Certificates
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.EnrollmentId == enrollment.Id || (c.StudentId == request.StudentId && c.CourseId == request.CourseId), cancellationToken);

        if (existingCert is not null)
        {
            return new CertificateDto(
                existingCert.Id,
                existingCert.Code,
                existingCert.EnrollmentId,
                existingCert.StudentId,
                existingCert.StudentName,
                existingCert.CourseId,
                existingCert.CourseName,
                existingCert.InstructorName,
                existingCert.PdfUrl,
                existingCert.IssuedAtUtc,
                existingCert.IsRevoked,
                existingCert.RevokedAtUtc,
                existingCert.RevocationReason);
        }

        // Fetch details for snapshot
        var student = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);

        var course = await _context.Courses
            .Include(c => c.Instructor)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (student is null || course is null)
        {
            return Error.NotFound("Certificate.DetailsNotFound", "Student or Course details not found.");
        }

        var studentName = student.FullName;
        var courseTitle = course.Title;
        var instructorName = course.Instructor?.FullName ?? "LearnHub Instructor";

        var certCode = $"CERT-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        var verificationUrl = $"{_options.VerificationBaseUrl.TrimEnd('/')}/{certCode}";

        var pdfModel = new CertificatePdfModel(
            certCode,
            studentName,
            courseTitle,
            instructorName,
            DateTimeOffset.UtcNow,
            verificationUrl);

        var pdfResult = await _pdfGenerator.GeneratePdfAsync(pdfModel, cancellationToken);
        if (pdfResult.IsError)
        {
            return pdfResult.Errors;
        }

        var fileName = $"{certCode}.pdf";
        var uploadResult = await _storageService.UploadRawFileAsync(pdfResult.Value, fileName, "certificates", "application/pdf", cancellationToken);
        var pdfUrl = uploadResult.IsSuccess ? uploadResult.Value : null;

        var certResult = Certificate.Create(
            Guid.NewGuid(),
            enrollment.Id,
            request.StudentId,
            request.CourseId,
            certCode,
            studentName,
            courseTitle,
            instructorName,
            pdfUrl);

        if (certResult.IsError)
        {
            return certResult.Errors;
        }

        var cert = certResult.Value;
        _context.Certificates.Add(cert);
        await _context.SaveChangesAsync(cancellationToken);

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
