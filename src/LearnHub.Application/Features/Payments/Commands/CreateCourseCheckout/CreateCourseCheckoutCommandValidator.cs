using FluentValidation;

namespace LearnHub.Application.Features.Payments.Commands.CreateCourseCheckout;

public sealed class CreateCourseCheckoutCommandValidator : AbstractValidator<CreateCourseCheckoutCommand>
{
    public CreateCourseCheckoutCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        RuleFor(x => x.SuccessUrl)
            .NotEmpty().WithMessage("Success URL is required.");

        RuleFor(x => x.CancelUrl)
            .NotEmpty().WithMessage("Cancel URL is required.");
    }
}
