using LearnHub.Application.common.Interfaces;
using LearnHub.Application.Features.Payments.Dtos;
using LearnHub.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Payments.Queries.GetUserPayments;

public sealed class GetUserPaymentsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetUserPaymentsQuery, Result<List<PaymentDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<PaymentDto>>> Handle(
        GetUserPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var coursePayments = await _context.Payments
            .Include(p => p.Order)
            .AsNoTracking()
            .Where(p => p.Order != null && p.Order.StudentId == request.StudentId)
            .Select(p => new PaymentDto(
                p.Id,
                p.Order!.StudentId,
                "CoursePurchase",
                p.ProviderReference,
                p.TransactionId,
                null,
                p.Amount.Amount,
                p.Amount.Currency,
                p.Status.ToString(),
                p.CreatedAtUtc,
                p.SucceededAtUtc,
                p.FailureReason))
            .ToListAsync(cancellationToken);

        var subPayments = await _context.SubscriptionPayments
            .Include(sp => sp.Subscription)
            .AsNoTracking()
            .Where(sp => sp.Subscription != null && sp.Subscription.StudentId == request.StudentId)
            .Select(sp => new PaymentDto(
                sp.Id,
                sp.Subscription!.StudentId,
                "SubscriptionPurchase",
                null,
                sp.GatewayTransactionId,
                null,
                sp.Amount.Amount,
                sp.Amount.Currency,
                sp.Status.ToString(),
                sp.CreatedAtUtc,
                sp.SucceededAtUtc,
                sp.FailureReason))
            .ToListAsync(cancellationToken);

        var allPayments = coursePayments
            .Concat(subPayments)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return allPayments;
    }
}
