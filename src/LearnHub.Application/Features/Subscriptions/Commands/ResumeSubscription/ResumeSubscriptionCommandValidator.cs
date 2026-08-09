using FluentValidation;

namespace LearnHub.Application.Features.Subscriptions.Commands.ResumeSubscription;

public sealed class ResumeSubscriptionCommandValidator : AbstractValidator<ResumeSubscriptionCommand>
{
    public ResumeSubscriptionCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");
    }
}
