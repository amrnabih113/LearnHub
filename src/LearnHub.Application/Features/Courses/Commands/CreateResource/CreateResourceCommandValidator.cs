using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.CreateResource;

public sealed class CreateResourceCommandValidator : AbstractValidator<CreateResourceCommand>
{
    public CreateResourceCommandValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        RuleFor(x => x.SizeInBytes).GreaterThanOrEqualTo(0);
    }
}