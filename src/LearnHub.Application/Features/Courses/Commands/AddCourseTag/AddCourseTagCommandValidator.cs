using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.AddCourseTag;

public sealed class AddCourseTagCommandValidator : AbstractValidator<AddCourseTagCommand>
{
    public AddCourseTagCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.TagId).NotEmpty();
    }
}