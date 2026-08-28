using System.Security.Claims;
using LearnHub.Application.Features.LearningPaths.Commands.AddCourseToLearningPath;
using LearnHub.Application.Features.LearningPaths.Commands.CreateLearningPath;
using LearnHub.Application.Features.LearningPaths.Commands.DeleteLearningPath;
using LearnHub.Application.Features.LearningPaths.Commands.PublishLearningPath;
using LearnHub.Application.Features.LearningPaths.Commands.RemoveCourseFromLearningPath;
using LearnHub.Application.Features.LearningPaths.Commands.ReorderLearningPathCourses;
using LearnHub.Application.Features.LearningPaths.Commands.UpdateLearningPath;
using LearnHub.Application.Features.LearningPaths.Queries.GetLearningPathById;
using LearnHub.Application.Features.LearningPaths.Queries.GetLearningPathProgress;
using LearnHub.Application.Features.LearningPaths.Queries.GetLearningPaths;
using LearnHub.Contracts.LearningPaths.Requests;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.LearningPaths.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/learning-paths")]
public sealed class LearningPathsController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetLearningPaths(
        [FromQuery] string? search,
        [FromQuery] CourseLevel? level,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var isAdminOrInstructor = User.IsInRole(nameof(Role.Admin)) || User.IsInRole(nameof(Role.Instructor));
        var statusFilter = isAdminOrInstructor ? (LearningPathStatus?)null : LearningPathStatus.Published;

        var query = new GetLearningPathsQuery(search, level, statusFilter, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLearningPathById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetLearningPathByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}/progress")]
    [Authorize]
    public async Task<IActionResult> GetLearningPathProgress(
        Guid id,
        CancellationToken cancellationToken)
    {
        var studentId = GetCurrentUserId();
        if (studentId == Guid.Empty)
        {
            return Unauthorized();
        }

        var query = new GetLearningPathProgressQuery(id, studentId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Instructor)}")]
    public async Task<IActionResult> CreateLearningPath(
        [FromBody] CreateLearningPathRequest request,
        CancellationToken cancellationToken)
    {
        var ownerId = GetCurrentUserId();
        var command = new CreateLearningPathCommand(
            request.Title,
            request.Slug,
            request.Description,
            request.ShortDescription,
            request.ThumbnailUrl,
            request.Level,
            ownerId == Guid.Empty ? null : ownerId);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Instructor)}")]
    public async Task<IActionResult> UpdateLearningPath(
        Guid id,
        [FromBody] UpdateLearningPathRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLearningPathCommand(
            id,
            request.Title,
            request.Slug,
            request.Description,
            request.ShortDescription,
            request.ThumbnailUrl,
            request.Level);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Instructor)}")]
    public async Task<IActionResult> PublishLearningPath(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new PublishLearningPathCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Instructor)}")]
    public async Task<IActionResult> DeleteLearningPath(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteLearningPathCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/courses")]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Instructor)}")]
    public async Task<IActionResult> AddCourse(
        Guid id,
        [FromBody] AddCourseToLearningPathRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCourseToLearningPathCommand(
            id,
            request.CourseId,
            request.TargetOrder,
            request.IsRequired);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/courses/{courseId:guid}")]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Instructor)}")]
    public async Task<IActionResult> RemoveCourse(
        Guid id,
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCourseFromLearningPathCommand(id, courseId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/courses/reorder")]
    [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Instructor)}")]
    public async Task<IActionResult> ReorderCourses(
        Guid id,
        [FromBody] ReorderLearningPathCoursesRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReorderLearningPathCoursesCommand(id, request.OrderedCourseIds);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }
}
