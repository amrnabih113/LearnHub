using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.DeleteResource;

public sealed class DeleteResourceCommandValidator : AbstractValidator<DeleteResourceCommand>
{
    public DeleteResourceCommandValidator()
    {
        RuleFor(x => x.ResourceId).NotEmpty();
    }
}