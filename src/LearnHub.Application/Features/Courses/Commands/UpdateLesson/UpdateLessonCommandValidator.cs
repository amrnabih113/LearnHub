using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.UpdateLesson;

public sealed class UpdateLessonCommandValidator : AbstractValidator<UpdateLessonCommand>
{
    public UpdateLessonCommandValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.VideoUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.DurationInMinutes).GreaterThan(0);
        RuleFor(x => x.Order).GreaterThan(0);
    }
}