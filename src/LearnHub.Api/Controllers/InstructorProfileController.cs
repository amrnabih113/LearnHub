using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[ApiController]
[Route("api/v1/instructors")]
public sealed class InstructorProfileController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpGet("{userId:guid}/profile")]
    public async Task<IActionResult> GetInstructorProfile(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new LearnHub.Application.Features.Instructor.Queries.GetInstructorProfile.GetInstructorProfileQuery(userId);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("profile")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateInstructorProfileRequest request,
        CancellationToken cancellationToken)
    {
        var instructorUserId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Instructor.Commands.UpdateInstructorProfile.UpdateInstructorProfileCommand(
            instructorUserId,
            request.ProfessionalTitle,
            request.Headline,
            request.Biography);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("profile/links")]
    [Authorize(Roles = $"{nameof(Role.Instructor)},{nameof(Role.Admin)}")]
    public async Task<IActionResult> AddLink(
        [FromBody] AddInstructorLinkRequest request,
        CancellationToken cancellationToken)
    {
        var instructorUserId = GetCurrentUserId();
        var command = new LearnHub.Application.Features.Instructor.Commands.AddInstructorLink.AddInstructorLinkCommand(
            instructorUserId,
            request.Title,
            request.Url);

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }
}

public sealed record UpdateInstructorProfileRequest(
    string? ProfessionalTitle = null,
    string? Headline = null,
    string? Biography = null);

public sealed record AddInstructorLinkRequest(
    string Title,
    string Url);
