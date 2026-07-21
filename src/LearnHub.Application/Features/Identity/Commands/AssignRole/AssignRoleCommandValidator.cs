namespace LearnHub.Application.Features.Identity.Commands.AssignRole;

using LearnHub.Domain.Common.Results;
using FluentValidation;

public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().IsInEnum();
        
    }
}