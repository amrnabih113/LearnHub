using LearnHub.Api.Extensions;
using LearnHub.Api.Files;
using LearnHub.Contracts.Courses.Requests;
using LearnHub.Application.Features.Courses.Commands.AddCourseTag;
using LearnHub.Application.Features.Courses.Commands.ChangeCourseStatus;
using LearnHub.Application.Features.Courses.Commands.CreateCourse;
using LearnHub.Application.Features.Courses.Commands.DeleteCourse;
using LearnHub.Application.Features.Courses.Commands.RemoveCourseTag;
using LearnHub.Application.Features.Courses.Commands.UpdateCourse;
using LearnHub.Application.Features.Courses.Queries.GetCourseById;
using LearnHub.Application.Features.Courses.Queries.GetCourseContent;
using LearnHub.Application.Features.Courses.Queries.GetCourses;
using LearnHub.Application.Features.Courses.Queries.GetCoursesByCategory;
using LearnHub.Application.Features.Courses.Queries.GetFeaturedCourses;
using LearnHub.Application.Features.Courses.Queries.GetInstructorCourses;
using LearnHub.Domain.Purchasing.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/courses")]
public sealed class CoursesController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCourse(
        [FromForm] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var priceResult = Money.Create(request.PriceAmount, string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency);
        if (priceResult.IsError)
        {
            return priceResult.Errors.ToProblem();
        }

        var command = new CreateCourseCommand(
            request.Title,
            request.Description,
            request.InstructorId,
            request.CategoryId,
            request.Thumbnail != null ? new FormFileData(request.Thumbnail) : null,
            request.Level,
            request.Status,
            priceResult.Value,
            request.IsIncludedInSubscription,
            request.RequiredSubscriptionTier,
            request.LanguageCode,
            request.LanguageName,
            request.Country);

        var result = await _sender.Send(command, cancellationToken);

        return HandleCreatedResult(result, nameof(GetCourseById), new { id = result.Value });
    }


    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateCourse(
        Guid id,
        [FromForm] UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var priceResult = Money.Create(request.PriceAmount, string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency);
        if (priceResult.IsError)
        {
            return priceResult.Errors.ToProblem();
        }

        var command = new UpdateCourseCommand(
            id,
            request.Title,
            request.Description,
            request.CategoryId,
            request.Thumbnail != null ? new FormFileData(request.Thumbnail) : null,
            request.Level,
            request.Status,
            priceResult.Value,
            request.IsIncludedInSubscription,
            request.RequiredSubscriptionTier,
            request.LanguageCode,
            request.LanguageName,
            request.Country);

        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteCourse(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCourseCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> ChangeCourseStatus(
        Guid id,
        [FromBody] ChangeCourseStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeCourseStatusCommand(id, request.Status);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{id:guid}/tags/{tagId:guid}")]
    [Authorize]
    public async Task<IActionResult> AddCourseTag(
        Guid id,
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var command = new AddCourseTagCommand(id, tagId);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}/tags/{tagId:guid}")]
    [Authorize]
    public async Task<IActionResult> RemoveCourseTag(
        Guid id,
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCourseTagCommand(id, tagId);
        var result = await _sender.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourses(
        [FromQuery] GetCoursesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeaturedCourses(
        [FromQuery] int count = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFeaturedCoursesQuery(count);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("category/{categoryId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoursesByCategory(
        Guid categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCoursesByCategoryQuery(categoryId, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("instructor/{instructorId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetInstructorCourses(
        Guid instructorId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetInstructorCoursesQuery(instructorId, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCourseByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}/content")]
    [Authorize]
    public async Task<IActionResult> GetCourseContent(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCourseContentQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return HandleResult(result);
    }
}
