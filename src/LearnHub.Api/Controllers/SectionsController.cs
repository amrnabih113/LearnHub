using LearnHub.Contracts.Courses.Requests;
using LearnHub.Application.Features.Courses.Commands.CreateSection;
using LearnHub.Application.Features.Courses.Commands.DeleteSection;
using LearnHub.Application.Features.Courses.Commands.UpdateSection;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[ApiController]
public sealed class SectionsController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost("api/v1/courses/{courseId:guid}/sections")]
    [Authorize]
    public async Task<IActionResult> CreateSection(
        Guid courseId,
        [FromBody] CreateSectionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSectionCommand(
            courseId,
            request.Title,
            request.Description,
            request.Order);

        var result = await _sender.Send(command, cancellationToken);

        return HandleCreatedResult(result);
    }


    [HttpPut("api/v1/sections/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateSection(
        Guid id,
        [FromBody] UpdateSectionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSectionCommand(
            id,
            request.Title,
            request.Description,
            request.Order);

        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("api/v1/sections/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteSection(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSectionCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
