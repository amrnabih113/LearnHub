using FluentValidation;

namespace LearnHub.Application.Features.Courses.Commands.DeleteSection;

public sealed class DeleteSectionCommandValidator : AbstractValidator<DeleteSectionCommand>
{
    public DeleteSectionCommandValidator()
    {
        RuleFor(x => x.SectionId)
            .NotEmpty();
    }
}