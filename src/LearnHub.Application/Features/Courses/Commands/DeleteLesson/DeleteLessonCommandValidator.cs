using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.DeleteLesson;

public sealed class DeleteLessonCommandValidator : AbstractValidator<DeleteLessonCommand>
{
    public DeleteLessonCommandValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
    }
}