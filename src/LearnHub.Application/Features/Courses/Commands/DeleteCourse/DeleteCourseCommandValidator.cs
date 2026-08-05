using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.DeleteCourse;

public sealed class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty();
    }
}