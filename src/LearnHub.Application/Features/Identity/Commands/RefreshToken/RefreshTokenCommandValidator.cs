using FluentValidation;

namespace LearnHub.Application.Features.Identity.Commands.RefreshToken;


public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");

        RuleFor(x => x.ExpiredToken)
            .NotEmpty().WithMessage("Expired token is required.");
    }
}