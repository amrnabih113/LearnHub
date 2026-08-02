using FluentValidation;

namespace LearnHub.Application.Features.Identity.Commands.VerifyForgotPasswordOtp;

public sealed class VerifyForgotPasswordOtpCommandValidator : AbstractValidator<VerifyForgotPasswordOtpCommand>
{
    public VerifyForgotPasswordOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Otp)
            .NotEmpty();
    }
}