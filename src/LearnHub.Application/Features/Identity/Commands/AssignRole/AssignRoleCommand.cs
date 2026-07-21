namespace LearnHub.Application.Features.Identity.Commands.AssignRole;

using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Identity;
using MediatR;


public sealed record AssignRoleCommand(
    string UserId,
    Role Role) : IRequest<Result<Updated>>; 