using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Subscriptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetSubscriptionsAdmin;

public sealed class GetSubscriptionsAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetSubscriptionsAdminQuery, Result<PagedResult<SubscriptionAdminSummaryDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<SubscriptionAdminSummaryDto>>> Handle(
        GetSubscriptionsAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Subscriptions
            .Include(s => s.Plan)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<SubscriptionStatus>(request.Status, true, out var statusEnum))
        {
            query = query.Where(s => s.Status == statusEnum);
        }

        if (request.StudentId.HasValue)
        {
            query = query.Where(s => s.StudentId == request.StudentId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var subsRaw = await query
            .OrderByDescending(s => s.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var studentIds = subsRaw.Select(s => s.StudentId).Distinct().ToList();
        var students = await _context.Users
            .AsNoTracking()
            .Where(u => studentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var items = subsRaw.Select(s =>
        {
            students.TryGetValue(s.StudentId, out var student);
            return new SubscriptionAdminSummaryDto(
                s.Id,
                s.StudentId,
                student?.FullName ?? string.Empty,
                student?.Email ?? string.Empty,
                s.Plan != null ? s.Plan.Tier.ToString() : s.Tier.ToString(),
                s.Status.ToString(),
                s.StartedAtUtc,
                s.ExpiresAtUtc,
                s.CreatedAtUtc);
        }).ToList();

        return new PagedResult<SubscriptionAdminSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
