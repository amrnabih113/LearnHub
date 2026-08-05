using LearnHub.Contracts.Courses.Requests;
using LearnHub.Application.Features.Courses.Commands.CreateResource;
using LearnHub.Application.Features.Courses.Commands.DeleteResource;
using LearnHub.Application.Features.Courses.Commands.UpdateResource;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[ApiController]
public sealed class ResourcesController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost("api/v1/lessons/{lessonId:guid}/resources")]
    [Authorize]
    public async Task<IActionResult> CreateResource(
        Guid lessonId,
        [FromBody] CreateResourceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateResourceCommand(
            lessonId,
            request.Name,
            request.Url,
            request.Type,
            request.SizeInBytes);

        var result = await _sender.Send(command, cancellationToken);

        return HandleCreatedResult(result);
    }


    [HttpPut("api/v1/resources/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateResource(
        Guid id,
        [FromBody] UpdateResourceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateResourceCommand(
            id,
            request.Name,
            request.Url,
            request.Type,
            request.SizeInBytes);

        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("api/v1/resources/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteResource(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteResourceCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
