using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.UpdateResource;

public sealed class UpdateResourceCommandValidator : AbstractValidator<UpdateResourceCommand>
{
    public UpdateResourceCommandValidator()
    {
        RuleFor(x => x.ResourceId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        RuleFor(x => x.SizeInBytes).GreaterThanOrEqualTo(0);
    }
}