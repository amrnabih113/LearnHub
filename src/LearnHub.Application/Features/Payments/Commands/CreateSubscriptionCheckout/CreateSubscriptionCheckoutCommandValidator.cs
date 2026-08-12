using FluentValidation;

namespace LearnHub.Application.Features.Payments.Commands.CreateSubscriptionCheckout;

public sealed class CreateSubscriptionCheckoutCommandValidator : AbstractValidator<CreateSubscriptionCheckoutCommand>
{
    public CreateSubscriptionCheckoutCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Student ID is required.");

        RuleFor(x => x.SubscriptionPlanId)
            .NotEmpty().WithMessage("Subscription Plan ID is required.");

        RuleFor(x => x.SuccessUrl)
            .NotEmpty().WithMessage("Success URL is required.");

        RuleFor(x => x.CancelUrl)
            .NotEmpty().WithMessage("Cancel URL is required.");
    }
}
