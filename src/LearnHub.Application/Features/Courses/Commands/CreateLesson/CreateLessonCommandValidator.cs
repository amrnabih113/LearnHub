using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.CreateLesson;

public sealed class CreateLessonCommandValidator : AbstractValidator<CreateLessonCommand>
{
    public CreateLessonCommandValidator()
    {
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.VideoUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.DurationInMinutes).GreaterThan(0);
        RuleFor(x => x.Order).GreaterThan(0);
    }
}