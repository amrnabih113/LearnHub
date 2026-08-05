using LearnHub.Contracts.Courses.Requests;
using LearnHub.Application.Features.Courses.Commands.CreateLesson;
using LearnHub.Application.Features.Courses.Commands.DeleteLesson;
using LearnHub.Application.Features.Courses.Commands.UpdateLesson;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[ApiController]
public sealed class LessonsController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost("api/v1/sections/{sectionId:guid}/lessons")]
    [Authorize]
    public async Task<IActionResult> CreateLesson(
        Guid sectionId,
        [FromBody] CreateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLessonCommand(
            sectionId,
            request.Title,
            request.Description,
            request.VideoUrl,
            request.IsPreview,
            request.Content,
            request.DurationInMinutes,
            request.Order);

        var result = await _sender.Send(command, cancellationToken);

        return HandleCreatedResult(result);
    }


    [HttpPut("api/v1/lessons/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateLesson(
        Guid id,
        [FromBody] UpdateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLessonCommand(
            id,
            request.Title,
            request.Description,
            request.VideoUrl,
            request.IsPreview,
            request.Content,
            request.DurationInMinutes,
            request.Order);

        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("api/v1/lessons/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteLesson(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteLessonCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }
}
