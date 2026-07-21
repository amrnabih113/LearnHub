namespace
    LearnHub.Application.Features.Identity.Commands.ResendVerificationEmail;

using FluentValidation;

public class SendVerificationEmailCommandValidator : AbstractValidator<SendVerificationEmailCommand>
{
    public SendVerificationEmailCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
