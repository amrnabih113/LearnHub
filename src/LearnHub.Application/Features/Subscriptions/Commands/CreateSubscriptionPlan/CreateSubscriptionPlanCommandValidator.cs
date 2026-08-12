using FluentValidation;

namespace LearnHub.Application.Features.Subscriptions.Commands.CreateSubscriptionPlan;

public sealed class CreateSubscriptionPlanCommandValidator : AbstractValidator<CreateSubscriptionPlanCommand>
{
    public CreateSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Plan name is required.")
            .MaximumLength(100).WithMessage("Plan name cannot exceed 100 characters.");

        RuleFor(x => x.Tier)
            .IsInEnum().WithMessage("Invalid subscription tier.");

        RuleFor(x => x.BillingCycle)
            .IsInEnum().WithMessage("Invalid billing cycle.");

        RuleFor(x => x.PriceAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Price amount must be greater than or equal to 0.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-character ISO code.");
    }
}
