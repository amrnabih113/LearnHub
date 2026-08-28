using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Commands.AssignRole;

public sealed record AssignRoleCommand(
    Guid AdminUserId,
    Guid TargetUserId,
    Role RoleToAssign) : IRequest<Result<Updated>>;

public sealed class AssignRoleCommandHandler(
    IAppDbContext context,
    ISecurityAuditService auditService)
    : IRequestHandler<AssignRoleCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ISecurityAuditService _auditService = auditService;

    public async Task<Result<Updated>> Handle(
        AssignRoleCommand request,
        CancellationToken cancellationToken)
    {
        var targetUser = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.TargetUserId, cancellationToken);

        if (targetUser is null)
        {
            return Error.NotFound("User.NotFound", "Target user was not found.");
        }

        var result = targetUser.AssignRole(request.RoleToAssign);
        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            request.AdminUserId,
            "RoleAssigned",
            request.TargetUserId,
            request.RoleToAssign.ToString(),
            $"Role {request.RoleToAssign} assigned to user {request.TargetUserId}",
            cancellationToken: cancellationToken);

        return Result.Updated;
    }
}
