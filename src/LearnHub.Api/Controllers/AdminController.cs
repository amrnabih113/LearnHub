using LearnHub.Application.Features.Admin.Commands.CreateCategory;
using LearnHub.Application.Features.Admin.Commands.CreateTag;
using LearnHub.Application.Features.Admin.Commands.DeleteCategory;
using LearnHub.Application.Features.Admin.Commands.DeleteTag;
using LearnHub.Application.Features.Admin.Commands.UpdateCategory;
using LearnHub.Application.Features.Admin.Commands.UpdateTag;
using LearnHub.Application.Features.Admin.Queries.GetAdminDashboard;
using LearnHub.Application.Features.Admin.Queries.GetCategoriesAdmin;
using LearnHub.Application.Features.Admin.Queries.GetCategoryByIdAdmin;
using LearnHub.Application.Features.Admin.Queries.GetCourseByIdAdmin;
using LearnHub.Application.Features.Admin.Queries.GetCoursesAdmin;
using LearnHub.Application.Features.Admin.Queries.GetOrderByIdAdmin;
using LearnHub.Application.Features.Admin.Queries.GetOrdersAdmin;
using LearnHub.Application.Features.Admin.Queries.GetPaymentByIdAdmin;
using LearnHub.Application.Features.Admin.Queries.GetPaymentsAdmin;
using LearnHub.Application.Features.Admin.Queries.GetReviewsAdmin;
using LearnHub.Application.Features.Admin.Queries.GetSubscriptionByIdAdmin;
using LearnHub.Application.Features.Admin.Queries.GetSubscriptionsAdmin;
using LearnHub.Application.Features.Admin.Queries.GetTagByIdAdmin;
using LearnHub.Application.Features.Admin.Queries.GetTagsAdmin;
using LearnHub.Application.Features.Admin.Queries.GetUserByIdAdmin;
using LearnHub.Application.Features.Admin.Queries.GetUsersAdmin;
using LearnHub.Application.Features.Courses.Commands.ChangeCourseStatus;
using LearnHub.Application.Features.Reviews.Commands.ModerateCourseReview;
using LearnHub.Contracts.Admin.Requests;
using LearnHub.Contracts.Admin.Responses;
using LearnHub.Contracts.Courses.Requests;
using LearnHub.Contracts.Reviews.Requests;
using LearnHub.Domain.Courses.Enums;
using LearnHub.Domain.Identity;
using LearnHub.Domain.Reviews.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/admin")]
[Authorize(Roles = nameof(Role.Admin))]
public sealed class AdminController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    #region Dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] string? range,
        CancellationToken cancellationToken)
    {
        var query = new GetAdminDashboardQuery(range);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Categories
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.Slug, request.Description, request.ParentCategoryId);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var response = new CategoryResponse(dto.Id, dto.Name, dto.Slug, dto.Description, dto.ParentCategoryId, dto.ParentCategoryName, dto.CreatedAtUtc);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoriesAdminQuery(search, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var items = result.Value.Items.Select(c => new CategoryResponse(
            c.Id, c.Name, c.Slug, c.Description, c.ParentCategoryId, c.ParentCategoryName, c.CreatedAtUtc)).ToList();

        return Ok(new
        {
            items,
            pageNumber = result.Value.PageNumber,
            pageSize = result.Value.PageSize,
            totalCount = result.Value.TotalCount
        });
    }

    [HttpGet("categories/{id:guid}")]
    public async Task<IActionResult> GetCategoryById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdAdminQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var response = new CategoryResponse(dto.Id, dto.Name, dto.Slug, dto.Description, dto.ParentCategoryId, dto.ParentCategoryName, dto.CreatedAtUtc);
        return Ok(response);
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(id, request.Name, request.Slug, request.Description, request.ParentCategoryId);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var response = new CategoryResponse(dto.Id, dto.Name, dto.Slug, dto.Description, dto.ParentCategoryId, dto.ParentCategoryName, dto.CreatedAtUtc);
        return Ok(response);
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Tags
    [HttpPost("tags")]
    public async Task<IActionResult> CreateTag(
        [FromBody] CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand(request.Name, request.Slug, request.Description);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var response = new TagResponse(dto.Id, dto.Name, dto.Slug, dto.Description, dto.CreatedAtUtc);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet("tags")]
    public async Task<IActionResult> GetTags(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTagsAdminQuery(search, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var items = result.Value.Items.Select(t => new TagResponse(
            t.Id, t.Name, t.Slug, t.Description, t.CreatedAtUtc)).ToList();

        return Ok(new
        {
            items,
            pageNumber = result.Value.PageNumber,
            pageSize = result.Value.PageSize,
            totalCount = result.Value.TotalCount
        });
    }

    [HttpGet("tags/{id:guid}")]
    public async Task<IActionResult> GetTagById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetTagByIdAdminQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var response = new TagResponse(dto.Id, dto.Name, dto.Slug, dto.Description, dto.CreatedAtUtc);
        return Ok(response);
    }

    [HttpPut("tags/{id:guid}")]
    public async Task<IActionResult> UpdateTag(
        Guid id,
        [FromBody] UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand(id, request.Name, request.Slug, request.Description);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsError)
        {
            return HandleResult(result);
        }

        var dto = result.Value;
        var response = new TagResponse(dto.Id, dto.Name, dto.Slug, dto.Description, dto.CreatedAtUtc);
        return Ok(response);
    }

    [HttpDelete("tags/{id:guid}")]
    public async Task<IActionResult> DeleteTag(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Users
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] string? role,
        [FromQuery] bool? isEmailVerified,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUsersAdminQuery(search, role, isEmailVerified, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> GetUserById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdAdminQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Courses
    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? instructorId,
        [FromQuery] Guid? categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCoursesAdminQuery(search, status, instructorId, categoryId, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("courses/{id:guid}")]
    public async Task<IActionResult> GetCourseById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCourseByIdAdminQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("courses/{id:guid}/status")]
    public async Task<IActionResult> ModerateCourseStatus(
        Guid id,
        [FromBody] ChangeCourseStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeCourseStatusCommand(id, request.Status);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Orders
    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] DateTimeOffset? fromDate,
        [FromQuery] DateTimeOffset? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOrdersAdminQuery(search, status, fromDate, toDate, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("orders/{id:guid}")]
    public async Task<IActionResult> GetOrderById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdAdminQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Payments
    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments(
        [FromQuery] string? status,
        [FromQuery] string? provider,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPaymentsAdminQuery(status, provider, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("payments/{id:guid}")]
    public async Task<IActionResult> GetPaymentById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdAdminQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Subscriptions
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptions(
        [FromQuery] string? tier,
        [FromQuery] string? status,
        [FromQuery] Guid? studentId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSubscriptionsAdminQuery(tier, status, studentId, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("subscriptions/{id:guid}")]
    public async Task<IActionResult> GetSubscriptionById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetSubscriptionByIdAdminQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }
    #endregion

    #region Reviews
    [HttpGet("reviews")]
    public async Task<IActionResult> GetReviews(
        [FromQuery] Guid? courseId,
        [FromQuery] Guid? studentId,
        [FromQuery] int? rating,
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetReviewsAdminQuery(courseId, studentId, rating, status, pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("reviews/{reviewId:guid}/status")]
    public async Task<IActionResult> ModerateReviewStatus(
        Guid reviewId,
        [FromBody] ModerateReviewRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ReviewStatus>(request.Status, true, out var reviewStatus))
        {
            return BadRequest(new { message = "Invalid review status value." });
        }

        var command = new ModerateCourseReviewCommand(reviewId, reviewStatus);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }
    #endregion
}
