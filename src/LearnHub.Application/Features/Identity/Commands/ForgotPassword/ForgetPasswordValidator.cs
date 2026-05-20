using FluentValidation;

namespace LearnHub.Application.Features.Identity.Commands.ForgotPassword;


public class ForgetPasswordValidator : AbstractValidator<ForgetPasswordCommand>
{
    public ForgetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");
    }
}