using FluentValidation;

namespace LearnHub.Application.Features.Subscriptions.Commands.ChangeSubscriptionPlan;

public sealed class ChangeSubscriptionPlanCommandValidator : AbstractValidator<ChangeSubscriptionPlanCommand>
{
    public ChangeSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");

        RuleFor(x => x.NewPlanId)
            .NotEmpty().WithMessage("New Subscription Plan ID is required.");
    }
}
