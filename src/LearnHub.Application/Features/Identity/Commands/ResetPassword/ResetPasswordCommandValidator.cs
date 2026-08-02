using FluentValidation;

namespace LearnHub.Application.Features.Identity.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.ResetToken)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(6);
    }
}