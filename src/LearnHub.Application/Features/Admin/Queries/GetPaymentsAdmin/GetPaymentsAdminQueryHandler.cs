using LearnHub.Application.common.Interfaces;
using LearnHub.Application.common.Models;
using LearnHub.Application.Features.Admin.Dtos;
using LearnHub.Domain.Common.Enums;
using LearnHub.Domain.Common.Results;
using LearnHub.Domain.Purchasing.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Application.Features.Admin.Queries.GetPaymentsAdmin;

public sealed class GetPaymentsAdminQueryHandler(IAppDbContext context)
    : IRequestHandler<GetPaymentsAdminQuery, Result<PagedResult<PaymentAdminSummaryDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PagedResult<PaymentAdminSummaryDto>>> Handle(
        GetPaymentsAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Payments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<PaymentStatus>(request.Status, true, out var statusEnum))
        {
            query = query.Where(p => p.Status == statusEnum);
        }

        if (!string.IsNullOrWhiteSpace(request.Provider) && Enum.TryParse<PaymentProvider>(request.Provider, true, out var providerEnum))
        {
            query = query.Where(p => p.Provider == providerEnum);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PaymentAdminSummaryDto(
                p.Id,
                p.OrderId,
                p.Provider.ToString(),
                p.Status.ToString(),
                p.Amount.Amount,
                p.Amount.Currency,
                p.TransactionId,
                p.ProviderReference,
                p.FailureReason,
                p.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<PaymentAdminSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
