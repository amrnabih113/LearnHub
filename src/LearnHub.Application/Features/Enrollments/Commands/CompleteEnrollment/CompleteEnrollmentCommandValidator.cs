using FluentValidation;

namespace LearnHub.Application.Features.Enrollments.Commands.CompleteEnrollment;

public sealed class CompleteEnrollmentCommandValidator : AbstractValidator<CompleteEnrollmentCommand>
{
    public CompleteEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");
    }
}
