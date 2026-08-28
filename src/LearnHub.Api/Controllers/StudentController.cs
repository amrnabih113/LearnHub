using System.Security.Claims;
using LearnHub.Application.Features.Certificates.Queries.GetStudentCertificates;
using LearnHub.Application.Features.Student.Commands.UpdateStudentProfile;
using LearnHub.Application.Features.Student.Queries.GetStudentLearningDashboard;
using LearnHub.Application.Features.Student.Queries.GetStudentOrderById;
using LearnHub.Application.Features.Student.Queries.GetStudentOrders;
using LearnHub.Application.Features.Student.Queries.GetStudentProfile;
using LearnHub.Application.Features.Student.Queries.GetStudentStatistics;
using LearnHub.Application.Features.Subscriptions.Queries.GetCurrentSubscription;
using LearnHub.Contracts.Certificates.Responses;
using LearnHub.Contracts.Student.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/student")]
[Authorize]
public sealed class StudentController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var query = new GetStudentProfileQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateStudentProfileRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var command = new UpdateStudentProfileCommand(
            studentId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.DateOfBirth,
            request.Bio,
            request.Country);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("my-learning")]
    public async Task<IActionResult> GetMyLearning(CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var query = new GetStudentLearningDashboardQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var query = new GetStudentStatisticsQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("certificates")]
    public async Task<IActionResult> GetCertificates(CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var query = new GetStudentCertificatesQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var responses = result.Value.Select(c => new CertificateResponse(
            c.Id, c.Code, c.EnrollmentId, c.StudentId, c.StudentName,
            c.CourseId, c.CourseTitle, c.InstructorName, c.PdfUrl, c.IssuedAtUtc)).ToList();

        return Ok(responses);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var query = new GetStudentOrdersQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("orders/{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var query = new GetStudentOrderByIdQuery(studentId, id);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("subscription")]
    public async Task<IActionResult> GetSubscription(CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        var query = new GetCurrentSubscriptionQuery(studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }
}
