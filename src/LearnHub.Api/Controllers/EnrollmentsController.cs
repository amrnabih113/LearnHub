using LearnHub.Contracts.Enrollments.Requests;
using LearnHub.Application.Features.Enrollments.Commands.CancelEnrollment;
using LearnHub.Application.Features.Enrollments.Commands.CompleteEnrollment;
using LearnHub.Application.Features.Enrollments.Commands.CreateEnrollment;
using LearnHub.Application.Features.Enrollments.Commands.SyncUserEnrollments;
using LearnHub.Application.Features.Enrollments.Commands.UpdateEnrollmentProgress;
using LearnHub.Application.Features.Enrollments.Queries.GetCourseAccess;
using LearnHub.Application.Features.Enrollments.Queries.GetCourseEnrollments;
using LearnHub.Application.Features.Enrollments.Queries.GetEnrollmentById;
using LearnHub.Application.Features.Enrollments.Queries.GetStudentEnrollments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/enrollments")]
public sealed class EnrollmentsController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateEnrollment(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateEnrollmentCommand(
            request.StudentId,
            request.CourseId);

        var result = await _sender.Send(command, cancellationToken);

        return HandleCreatedResult(result, nameof(GetEnrollmentById), new { id = result.Value });
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetEnrollmentById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetEnrollmentByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("courses/{courseId:guid}/access")]
    [Authorize]
    public async Task<IActionResult> GetCourseAccess(
        Guid courseId,
        [FromQuery] Guid studentId,
        CancellationToken cancellationToken)
    {
        var query = new GetCourseAccessQuery(courseId, studentId);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("sync")]
    [Authorize]
    public async Task<IActionResult> SyncUserEnrollments(
        [FromQuery] Guid studentId,
        CancellationToken cancellationToken)
    {
        var command = new SyncUserEnrollmentsCommand(studentId);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("student/{studentId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetStudentEnrollments(
        Guid studentId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetStudentEnrollmentsQuery(studentId, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("course/{courseId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetCourseEnrollments(
        Guid courseId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCourseEnrollmentsQuery(courseId, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{id:guid}/progress")]
    [Authorize]
    public async Task<IActionResult> UpdateProgress(
        Guid id,
        [FromBody] UpdateEnrollmentProgressRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEnrollmentProgressCommand(
            id,
            request.LessonId,
            request.WatchDurationSeconds,
            request.TotalLessons,
            request.LessonDurationSeconds);

        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{id:guid}/complete")]
    [Authorize]
    public async Task<IActionResult> Complete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CompleteEnrollmentCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CancelEnrollmentCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
