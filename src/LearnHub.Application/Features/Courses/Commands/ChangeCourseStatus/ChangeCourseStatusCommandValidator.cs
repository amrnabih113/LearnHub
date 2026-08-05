using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.ChangeCourseStatus;

public sealed class ChangeCourseStatusCommandValidator : AbstractValidator<ChangeCourseStatusCommand>
{
    public ChangeCourseStatusCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}