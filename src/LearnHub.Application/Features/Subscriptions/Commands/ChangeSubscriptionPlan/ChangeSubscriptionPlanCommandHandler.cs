using LearnHub.Application.common.Interfaces;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Subscriptions.Commands.ChangeSubscriptionPlan;

public sealed class ChangeSubscriptionPlanCommandHandler(
    IAppDbContext context,
    ICourseAccessService courseAccessService)
    : IRequestHandler<ChangeSubscriptionPlanCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ICourseAccessService _courseAccessService = courseAccessService;

    public async Task<Result<Updated>> Handle(
        ChangeSubscriptionPlanCommand request,
        CancellationToken cancellationToken)
    {
        var plan = await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.NewPlanId && p.IsActive, cancellationToken);

        if (plan is null)
        {
            return Error.NotFound("SubscriptionPlan.NotFound", "Selected subscription plan not found or inactive.");
        }

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

        if (subscription is null)
        {
            var now = DateTimeOffset.UtcNow;
            var expiresAt = plan.BillingCycle == BillingCycle.Monthly ? now.AddMonths(1) : now.AddYears(1);

            var createResult = Subscription.Create(Guid.NewGuid(), request.StudentId, plan.Tier, plan.BillingCycle, now, expiresAt);
            if (createResult.IsError)
            {
                return createResult.Errors;
            }

            subscription = createResult.Value;
            subscription.Activate(now);
            _context.Subscriptions.Add(subscription);
        }
        else
        {
            subscription.UpgradeTier(plan.Tier, DateTimeOffset.UtcNow);
            subscription.Activate(DateTimeOffset.UtcNow);
        }

        _context.Entry(subscription).Property(s => s.SubscriptionPlanId).CurrentValue = plan.Id;
        await _context.SaveChangesAsync(cancellationToken);

        await _courseAccessService.SynchronizeUserEnrollmentsAsync(request.StudentId, cancellationToken);

        return Result.Updated;
    }
}
