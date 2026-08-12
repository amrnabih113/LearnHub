using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Subscriptions.Dtos;
using LearnHub.Application.Features.Subscriptions.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Subscriptions.Queries.GetSubscriptionPlanById;

public sealed class GetSubscriptionPlanByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetSubscriptionPlanByIdQuery, Result<SubscriptionPlanDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<SubscriptionPlanDto>> Handle(
        GetSubscriptionPlanByIdQuery request,
        CancellationToken cancellationToken)
    {
        var plan = await _context.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (plan is null)
        {
            return Error.NotFound("SubscriptionPlan.NotFound", "Subscription plan not found.");
        }

        return plan.ToDto();
    }
}
