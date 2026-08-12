using FluentValidation;

namespace LearnHub.Application.Features.Enrollments.Commands.UpdateEnrollmentProgress;

public sealed class UpdateEnrollmentProgressCommandValidator : AbstractValidator<UpdateEnrollmentProgressCommand>
{
    public UpdateEnrollmentProgressCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");

        RuleFor(x => x.LessonId)
            .NotEmpty().WithMessage("Lesson ID is required.");

        RuleFor(x => x.WatchDurationSeconds)
            .GreaterThanOrEqualTo(0).WithMessage("Watch duration cannot be negative.");

        RuleFor(x => x.TotalLessons)
            .GreaterThan(0).WithMessage("Total lessons must be greater than zero.");
    }
}
