using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Application.Features.Reviews.Commands.CreateInstructorReview;
using LearnHub.Application.Features.Reviews.Commands.DeleteInstructorReview;
using LearnHub.Application.Features.Reviews.Commands.UpdateInstructorReview;
using LearnHub.Application.Features.Reviews.Queries.GetInstructorReviews;
using LearnHub.Application.Features.Reviews.Queries.GetInstructorReviewSummary;
using LearnHub.Application.Features.Reviews.Queries.GetStudentInstructorReview;
using LearnHub.Contracts.Reviews.Requests;
using LearnHub.Contracts.Reviews.Responses;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/instructors/{instructorId:guid}/reviews")]
public sealed class InstructorReviewsController(
    ISender sender,
    ICurrentUserService currentUserService) : BaseController
{
    private readonly ISender _sender = sender;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview(
        Guid instructorId,
        [FromBody] CreateInstructorReviewRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new CreateInstructorReviewCommand(instructorId, studentId, request.Rating, request.Comment, request.CourseId);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToInstructorReviewResponse(result.Value);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetInstructorReviews(
        Guid instructorId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetInstructorReviewsQuery(instructorId, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var items = result.Value.Items.Select(MapToInstructorReviewResponse).ToList();
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
    public async Task<IActionResult> GetInstructorReviewSummary(
        Guid instructorId,
        CancellationToken cancellationToken)
    {
        var query = new GetInstructorReviewSummaryQuery(instructorId);
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
        Guid instructorId,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var query = new GetStudentInstructorReviewQuery(instructorId, studentId);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        if (result.Value is null)
        {
            return NotFound();
        }

        var response = MapToInstructorReviewResponse(result.Value);
        return Ok(response);
    }

    [HttpPut("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(
        Guid instructorId,
        Guid reviewId,
        [FromBody] UpdateInstructorReviewRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var command = new UpdateInstructorReviewCommand(reviewId, studentId, request.Rating, request.Comment);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var response = MapToInstructorReviewResponse(result.Value);
        return Ok(response);
    }

    [HttpDelete("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(
        Guid instructorId,
        Guid reviewId,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId ?? Guid.Empty;
        var isAdminOrInstructor = User.IsInRole(Role.Admin.ToString()) || User.IsInRole(Role.Instructor.ToString());

        var command = new DeleteInstructorReviewCommand(reviewId, studentId, isAdminOrInstructor);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    private static InstructorReviewResponse MapToInstructorReviewResponse(Application.Features.Reviews.Dtos.InstructorReviewDto dto)
    {
        return new InstructorReviewResponse(
            dto.Id,
            dto.InstructorId,
            dto.StudentId,
            dto.StudentName,
            dto.StudentImageUrl,
            dto.CourseId,
            dto.Rating,
            dto.Comment,
            dto.Status,
            dto.CreatedAtUtc);
    }
}
