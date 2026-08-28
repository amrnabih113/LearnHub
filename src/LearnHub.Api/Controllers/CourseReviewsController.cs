using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Features.Reviews.Commands.CreateCourseReview;
using LearnHub.Application.Features.Reviews.Commands.DeleteCourseReview;
using LearnHub.Application.Features.Reviews.Commands.ModerateCourseReview;
using LearnHub.Application.Features.Reviews.Commands.UpdateCourseReview;
using LearnHub.Application.Features.Reviews.Queries.GetCourseReviews;
using LearnHub.Application.Features.Reviews.Queries.GetCourseReviewSummary;
using LearnHub.Application.Features.Reviews.Queries.GetStudentCourseReview;
using LearnHub.Contracts.Reviews.Requests;
using LearnHub.Contracts.Reviews.Responses;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Reviews.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/courses/{courseId:guid}/reviews")]
public sealed class CourseReviewsController(
    ISender sender,
    ICurrentUserService currentUserService) : BaseController
{
    private readonly ISender _sender = sender;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview(
        Guid courseId,
        [FromBody] CreateCourseReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Rating < 1 || request.Rating > 5)
        {
            return BadRequest(new { message = "Rating must be between 1 and 5." });
        }

        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new CreateCourseReviewCommand(courseId, studentId, request.Rating, request.Comment);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToCourseReviewResponse(result.Value);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseReviews(
        Guid courseId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCourseReviewsQuery(courseId, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var items = result.Value.Items.Select(MapToCourseReviewResponse).ToList();
        return Ok(new
        {
            items,
            pageNumber = result.Value.PageNumber,
            pageSize = result.Value.PageSize,
            totalCount = result.Value.TotalCount
        });
    }

    [HttpGet("summary")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseReviewSummary(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var query = new GetCourseReviewSummaryQuery(courseId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = new ReviewSummaryResponse(
            result.Value.AverageRating,
            result.Value.TotalReviews,
            result.Value.StarCounts);

        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyReview(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var query = new GetStudentCourseReviewQuery(courseId, studentId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        if (result.Value is null)
        {
            return NotFound();
        }

        var response = MapToCourseReviewResponse(result.Value);
        return Ok(response);
    }

    [HttpPut("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(
        Guid courseId,
        Guid reviewId,
        [FromBody] UpdateCourseReviewRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new UpdateCourseReviewCommand(reviewId, studentId, request.Rating, request.Comment);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToCourseReviewResponse(result.Value);
        return Ok(response);
    }

    [HttpDelete("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(
        Guid courseId,
        Guid reviewId,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var isAdminOrInstructor = User.IsInRole(Role.Admin.ToString()) || User.IsInRole(Role.Instructor.ToString());

        var command = new DeleteCourseReviewCommand(reviewId, studentId, isAdminOrInstructor);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{reviewId:guid}/status")]
    [Authorize(Roles = "Admin,Instructor")]
    public async Task<IActionResult> ModerateReview(
        Guid courseId,
        Guid reviewId,
        [FromBody] ModerateReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ReviewStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { message = "Invalid review status value." });
        }

        var command = new ModerateCourseReviewCommand(reviewId, status);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    private static CourseReviewResponse MapToCourseReviewResponse(Application.Features.Reviews.Dtos.CourseReviewDto dto)
    {
        return new CourseReviewResponse(
            dto.Id,
            dto.CourseId,
            dto.StudentId,
            dto.StudentName,
            dto.StudentImageUrl,
            dto.Rating,
            dto.Comment,
            dto.Status,
            dto.CreatedAtUtc);
    }
}
