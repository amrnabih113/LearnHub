using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Commands.ApproveInstructor;

public sealed record ApproveInstructorCommand(
    Guid AdminUserId,
    Guid InstructorUserId) : IRequest<Result<Updated>>;

public sealed class ApproveInstructorCommandHandler(
    IAppDbContext context,
    ISecurityAuditService auditService)
    : IRequestHandler<ApproveInstructorCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ISecurityAuditService _auditService = auditService;

    public async Task<Result<Updated>> Handle(
        ApproveInstructorCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await _context.InstructorProfiles
            .FirstOrDefaultAsync(p => p.UserId == request.InstructorUserId, cancellationToken);

        if (profile is null)
        {
            return Error.NotFound("InstructorProfile.NotFound", "Instructor profile was not found.");
        }

        var result = profile.Approve();
        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            request.AdminUserId,
            "InstructorApproved",
            request.InstructorUserId,
            profile.Id.ToString(),
            $"Instructor profile {profile.Id} approved by Admin {request.AdminUserId}",
            cancellationToken: cancellationToken);

        return Result.Updated;
    }
}
