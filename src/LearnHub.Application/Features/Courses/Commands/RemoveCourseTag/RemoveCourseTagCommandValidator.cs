using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.RemoveCourseTag;

public sealed class RemoveCourseTagCommandValidator : AbstractValidator<RemoveCourseTagCommand>
{
    public RemoveCourseTagCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.TagId).NotEmpty();
    }
}