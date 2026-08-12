using LearnHub.Application.common.Errors;
using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Payments.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Payments.Commands.CreateSubscriptionCheckout;

public sealed class CreateSubscriptionCheckoutCommandHandler(
    IAppDbContext context,
    IPaymentGatewayService paymentGatewayService)
    : IRequestHandler<CreateSubscriptionCheckoutCommand, Result<CheckoutSessionDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IPaymentGatewayService _paymentGatewayService = paymentGatewayService;

    public async Task<Result<CheckoutSessionDto>> Handle(
        CreateSubscriptionCheckoutCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.StudentId, cancellationToken);
        if (user is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var plan = await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.SubscriptionPlanId && p.IsActive, cancellationToken);
        if (plan is null)
        {
            return Error.NotFound("SubscriptionPlan.NotFound", "Subscription plan not found or inactive.");
        }

        var now = DateTimeOffset.UtcNow;
        var expiresAt = plan.BillingCycle == BillingCycle.Monthly ? now.AddMonths(1) : now.AddYears(1);

        var subResult = Subscription.Create(Guid.NewGuid(), user.Id, plan.Tier, plan.BillingCycle, now, expiresAt);
        if (subResult.IsError)
        {
            return subResult.Errors;
        }

        var subscription = subResult.Value;

        var subPaymentResult = SubscriptionPayment.Create(Guid.NewGuid(), subscription.Id, plan.Price, now.AddMinutes(10));
        if (subPaymentResult.IsError)
        {
            return subPaymentResult.Errors;
        }

        var subscriptionPayment = subPaymentResult.Value;
        subscriptionPayment.MarkProcessing();

        _context.Subscriptions.Add(subscription);
        _context.Entry(subscription).Property(s => s.SubscriptionPlanId).CurrentValue = plan.Id;
        _context.SubscriptionPayments.Add(subscriptionPayment);
        await _context.SaveChangesAsync(cancellationToken);


        var args = new CreateCheckoutSessionArgs(
            UserId: user.Id,
            UserEmail: user.Email,
            PaymentType: PaymentType.SubscriptionPurchase,
            TargetId: plan.Id,
            ItemTitle: $"{plan.Name} ({plan.Tier})",
            Amount: plan.Price.Amount,
            Currency: plan.Price.Currency,
            SuccessUrl: request.SuccessUrl,
            CancelUrl: request.CancelUrl,
            Metadata: new Dictionary<string, string>
            {
                ["subscriptionId"] = subscription.Id.ToString(),
                ["subscriptionPaymentId"] = subscriptionPayment.Id.ToString(),
                ["studentId"] = user.Id.ToString(),
                ["planId"] = plan.Id.ToString()
            });

        var sessionResult = await _paymentGatewayService.CreateCheckoutSessionAsync(args, cancellationToken);
        if (sessionResult.IsError)
        {
            return sessionResult.Errors;
        }

        return new CheckoutSessionDto(sessionResult.Value.SessionId, sessionResult.Value.CheckoutUrl);
    }
}
