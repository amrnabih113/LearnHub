using LearnHub.Api.Extensions;
using LearnHub.Api.Files;
using LearnHub.Contracts.Courses.Requests;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class LessonsController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;
    [HttpPost("sections/{sectionId:guid}/lessons")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> CreateLesson(
        Guid sectionId,
        [FromBody] CreateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Lessons.Commands.CreateLesson.CreateLessonCommand(
            sectionId, instructorId, request.Title, request.Description, request.Content, request.IsPreview, request.Order);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("lessons/{id:guid}")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> UpdateLesson(
        Guid id,
        [FromBody] UpdateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Lessons.Commands.UpdateLesson.UpdateLessonCommand(
            id, instructorId, request.Title, request.Description, request.Content, request.IsPreview);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("lessons/{id:guid}")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> DeleteLesson(
        Guid id,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Lessons.Commands.DeleteLesson.DeleteLessonCommand(id, instructorId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("sections/{sectionId:guid}/lessons/reorder")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> ReorderLessons(
        Guid sectionId,
        [FromBody] ReorderLessonsRequest request,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var items = request.Items.Select(i => new LearnHub.Application.Features.Lessons.Commands.ReorderLessons.LessonOrderItem(i.LessonId, i.Order)).ToList();
        var command = new LearnHub.Application.Features.Lessons.Commands.ReorderLessons.ReorderLessonsCommand(sectionId, instructorId, items);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("lessons/{id:guid}/video")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> UploadLessonVideo(
        Guid id,
        IFormFile file,
        [FromForm] int durationInMinutes = 0,
        CancellationToken cancellationToken = default)
    {
        var instructorId = GetCurrentUserId();
        var fileData = new FormFileData(file);
        var command = new LearnHub.Application.Features.Lessons.Commands.UploadLessonVideo.UploadLessonVideoCommand(id, instructorId, fileData, durationInMinutes);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("lessons/{id:guid}/publish")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> PublishLesson(
        Guid id,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Lessons.Commands.PublishLesson.PublishLessonCommand(id, instructorId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }
}
