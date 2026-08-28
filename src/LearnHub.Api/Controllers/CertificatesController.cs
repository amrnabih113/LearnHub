using System.Security.Claims;
using LearnHub.Application.Features.Certificates.Commands.IssueCertificate;
using LearnHub.Application.Features.Certificates.Queries.GetCertificateById;
using LearnHub.Application.Features.Certificates.Queries.GetStudentCertificates;
using LearnHub.Application.Features.Certificates.Queries.VerifyCertificate;
using LearnHub.Contracts.Certificates.Requests;
using LearnHub.Contracts.Certificates.Responses;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/certificates")]
public sealed class CertificatesController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> IssueCertificate(
        [FromBody] IssueCertificateRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        if (studentId == Guid.Empty)
        {
            return Unauthorized();
        }

        var command = new IssueCertificateCommand(studentId, request.CourseId);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var response = new CertificateResponse(
            dto.Id, dto.Code, dto.EnrollmentId, dto.StudentId, dto.StudentName,
            dto.CourseId, dto.CourseTitle, dto.InstructorName, dto.PdfUrl, dto.IssuedAtUtc);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMyCertificates(CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        if (studentId == Guid.Empty)
        {
            return Unauthorized();
        }

        var query = new GetStudentCertificatesQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var items = result.Value.Select(c => new CertificateResponse(
            c.Id, c.Code, c.EnrollmentId, c.StudentId, c.StudentName,
            c.CourseId, c.CourseTitle, c.InstructorName, c.PdfUrl, c.IssuedAtUtc)).ToList();

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetCertificateById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole(nameof(Role.Admin));

        var query = new GetCertificateByIdQuery(id, userId, isAdmin);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var response = new CertificateResponse(
            dto.Id, dto.Code, dto.EnrollmentId, dto.StudentId, dto.StudentName,
            dto.CourseId, dto.CourseTitle, dto.InstructorName, dto.PdfUrl, dto.IssuedAtUtc);

        return Ok(response);
    }

    [HttpGet("{id:guid}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadCertificate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole(nameof(Role.Admin));

        var query = new GetCertificateByIdQuery(id, userId, isAdmin);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var cert = result.Value;
        if (string.IsNullOrWhiteSpace(cert.PdfUrl))
        {
            return NotFound(new { message = "Certificate PDF file URL not found." });
        }

        return Ok(new { downloadUrl = cert.PdfUrl, code = cert.Code });
    }

    [HttpGet("{code}/verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyCertificate(
        string code,
        CancellationToken cancellationToken)
    {
        var query = new VerifyCertificateQuery(code);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var response = new CertificateVerificationResponse(
            dto.IsValid, dto.Code, dto.StudentName, dto.CourseTitle, dto.InstructorName, dto.IssuedAtUtc, dto.Status,
            dto.RevokedAtUtc, dto.RevocationReason);

        return Ok(response);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }
}
