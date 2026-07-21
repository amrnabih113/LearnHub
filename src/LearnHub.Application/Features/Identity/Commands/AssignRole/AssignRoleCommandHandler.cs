namespace LearnHub.Application.Features.Identity.Commands.AssignRole;

using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Common.Interfaces.Authentication;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;


public class AssignRoleCommandHandler(IAppDbContext context, ICurrentUserService currentUser) : IRequestHandler<AssignRoleCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {

        if (currentUser.Role != Role.Admin)
        {
            return ApplicationErrors.AdminRoleUnauthorized;
        }
        var user = await context.Users.FindAsync(new object?[] { request.UserId }, cancellationToken);
        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }   
        if (user.Roles.Contains(request.Role))
        {
            return ApplicationErrors.RoleAlreadyAssigned;
        }
        if (!Enum.IsDefined(request.Role))
        {
            return ApplicationErrors.InvalidRole;
        }
        var result = user.AssignRole(request.Role);

        if (result.IsError)
        {
            return result.Errors;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}