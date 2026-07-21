using FluentValidation;

namespace LearnHub.Application.Features.Identity.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Date of birth must be in the past.");
        RuleFor(x => x.Bio).MaximumLength(1000);
        RuleFor(x => x.PhoneNumber).MaximumLength(20).ChildRules(phone =>
        {
            phone.RuleFor(x => x).Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x)).WithMessage("Phone number must be in E.164 format.");
        });
        RuleFor(x => x.Country).MaximumLength(100);
    }
}