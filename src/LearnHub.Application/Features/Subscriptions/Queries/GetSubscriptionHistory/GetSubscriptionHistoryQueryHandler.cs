using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Subscriptions.Dtos;
using LearnHub.Application.Features.Subscriptions.Mappers;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Subscriptions.Queries.GetSubscriptionHistory;

public sealed class GetSubscriptionHistoryQueryHandler(IAppDbContext context)
    : IRequestHandler<GetSubscriptionHistoryQuery, Result<List<SubscriptionDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<SubscriptionDto>>> Handle(
        GetSubscriptionHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var subscriptions = await _context.Subscriptions
            .Include(s => s.Plan)
            .AsNoTracking()
            .Where(s => s.StudentId == request.StudentId)
            .OrderByDescending(s => s.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return subscriptions.Select(s => s.ToDto()).ToList();
    }
}
