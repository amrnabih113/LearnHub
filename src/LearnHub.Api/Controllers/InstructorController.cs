using System.Security.Claims;
using LearnHub.Application.Features.Courses.Queries.GetCourses;
using LearnHub.Application.Features.Instructor.Queries.GetInstructorAnalytics;
using LearnHub.Application.Features.Instructor.Queries.GetInstructorDashboard;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[Route("api/v1/instructor")]
[Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
public sealed class InstructorController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var query = new GetInstructorDashboardQuery(instructorId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics(
        [FromQuery] Guid? courseId,
        CancellationToken cancellationToken)
    {
        var instructorId = GetCurrentUserId();
        var query = new GetInstructorAnalyticsQuery(instructorId, courseId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("courses")]
    public async Task<IActionResult> GetMyCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var instructorId = GetCurrentUserId();
        var query = new GetCoursesQuery(
            CategoryId: null,
            InstructorId: instructorId,
            Level: null,
            Status: null,
            Language: null,
            MinPrice: null,
            MaxPrice: null,
            PageNumber: pageNumber,
            PageSize: pageSize);

        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }
}
