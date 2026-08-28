using LearnHub.Contracts.Courses.Requests;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[ApiController]
public sealed class SectionsController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost("api/v1/courses/{courseId:guid}/sections")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> CreateSection(
        Guid courseId,
        [FromBody] CreateSectionRequest request,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Sections.Commands.CreateSection.CreateSectionCommand(
            courseId,
            instructorId,
            request.Title,
            request.Description,
            request.Order);

        var result = await _sender.Send(command, cancellationToken);

        return HandleCreatedResult(result);
    }

    [HttpPut("api/v1/sections/{id:guid}")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> UpdateSection(
        Guid id,
        [FromBody] UpdateSectionRequest request,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Sections.Commands.UpdateSection.UpdateSectionCommand(
            id,
            instructorId,
            request.Title,
            request.Description);

        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("api/v1/sections/{id:guid}")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> DeleteSection(
        Guid id,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Sections.Commands.DeleteSection.DeleteSectionCommand(id, instructorId);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("api/v1/courses/{courseId:guid}/sections/reorder")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> ReorderSections(
        Guid courseId,
        [FromBody] ReorderSectionsRequest request,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var items = request.Items.Select(i => new LearnHub.Application.Features.Sections.Commands.ReorderSections.SectionOrderItem(i.SectionId, i.Order)).ToList();
        var command = new LearnHub.Application.Features.Sections.Commands.ReorderSections.ReorderSectionsCommand(courseId, instructorId, items);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("api/v1/sections/{id:guid}/publish")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> PublishSection(
        Guid id,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Sections.Commands.PublishSection.PublishSectionCommand(id, instructorId);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
