using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Subscriptions.Dtos;
using LearnHub.Application.Features.Subscriptions.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Subscriptions.Queries.GetSubscriptionPlans;

public sealed class GetSubscriptionPlansQueryHandler(IAppDbContext context)
    : IRequestHandler<GetSubscriptionPlansQuery, Result<List<SubscriptionPlanDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<SubscriptionPlanDto>>> Handle(
        GetSubscriptionPlansQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.SubscriptionPlans.AsNoTracking();

        if (request.OnlyActive)
        {
            query = query.Where(p => p.IsActive);
        }

        var plans = await query
            .OrderBy(p => p.Tier)
            .ThenBy(p => p.Price.Amount)
            .ToListAsync(cancellationToken);

        return plans.Select(p => p.ToDto()).ToList();
    }
}
