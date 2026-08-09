using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Subscriptions.Dtos;
using LearnHub.Application.Features.Subscriptions.Mappers;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Subscriptions.Queries.GetCurrentSubscription;

public sealed class GetCurrentSubscriptionQueryHandler(IAppDbContext context)
    : IRequestHandler<GetCurrentSubscriptionQuery, Result<SubscriptionDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<SubscriptionDto>> Handle(
        GetCurrentSubscriptionQuery request,
        CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId, cancellationToken);

        if (subscription is null)
        {
            return new SubscriptionDto(
                Id: Guid.Empty,
                StudentId: request.StudentId,
                Tier: SubscriptionTier.Free,
                SubscriptionPlanId: Guid.Empty,
                PlanName: "Free Default Plan",
                BillingCycle: BillingCycle.Monthly,
                Status: SubscriptionStatus.Active,
                StartedAtUtc: DateTimeOffset.UtcNow,
                ExpiresAtUtc: DateTimeOffset.MaxValue,
                TrialEndsAtUtc: null,
                CancelledAtUtc: null,
                AutoRenewEnabled: false);
        }

        return subscription.ToDto();
    }
}
