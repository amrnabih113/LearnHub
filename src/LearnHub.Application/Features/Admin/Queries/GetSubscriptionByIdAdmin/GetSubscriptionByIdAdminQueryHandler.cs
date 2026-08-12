using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetSubscriptionByIdAdmin;

public sealed class GetSubscriptionByIdAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetSubscriptionByIdAdminQuery, Result<SubscriptionAdminDetailDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<SubscriptionAdminDetailDto>> Handle(
        GetSubscriptionByIdAdminQuery request,
        CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Plan)
            .Include(s => s.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (subscription is null)
        {
            return Error.NotFound("Subscription.NotFound", "Subscription not found.");
        }

        var student = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == subscription.StudentId, cancellationToken);

        var payments = subscription.Payments.Select(p => new SubscriptionPaymentAdminDto(
            p.Id,
            p.Amount.Amount,
            p.Amount.Currency,
            p.Status.ToString(),
            p.GatewayTransactionId,
            p.FailureReason,
            p.CreatedAtUtc)).ToList();

        return new SubscriptionAdminDetailDto(
            subscription.Id,
            subscription.StudentId,
            student?.FullName ?? string.Empty,
            student?.Email ?? string.Empty,
            subscription.Plan != null ? subscription.Plan.Tier.ToString() : subscription.Tier.ToString(),
            subscription.Status.ToString(),
            subscription.StartedAtUtc,
            subscription.ExpiresAtUtc,
            subscription.CancelledAtUtc,
            payments,
            subscription.CreatedAtUtc);
    }
}
